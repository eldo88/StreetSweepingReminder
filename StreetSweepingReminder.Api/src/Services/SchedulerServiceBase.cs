using FluentResults;
using StreetSweepingReminder.Api.Errors;
using StreetSweepingReminder.Api.Repositories;
using StreetSweepingReminder.Api.Services.Utils;

namespace StreetSweepingReminder.Api.Services;

public abstract class SchedulerServiceBase<TCommand, TEntity, TRepository, TParentId, TDerivedService> 
    where TEntity: class
    where TRepository: ICrudRepository<TEntity>
{
    protected readonly ILogger<TDerivedService> _logger;
    protected readonly TRepository _repository;
    
    protected SchedulerServiceBase(ILogger<TDerivedService> logger, TRepository repository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    protected abstract bool GetIsRecurring(TCommand command);
    protected abstract DateTime GetBaseScheduleDate(TCommand command);
    protected abstract object[] GetRecurringParameters(TCommand command);
    
    
    /// <summary>
    /// Creates the entity instance from the command and parent ID (without the date initially).
    /// </summary>
    protected abstract TEntity MapToEntity(TCommand command, TParentId parentId);
    
    /// <summary>
    /// Sets the specific schedule date property on the entity instance.
    /// </summary>
    protected abstract void SetScheduleDateOnEntity(TEntity entity, DateTime scheduleDate);
    
    /// <summary>
    /// Allows derived classes to set specific properties (like IsActive) just before saving.
    /// </summary>
    protected virtual void CustomizeEntityBeforeSave(TEntity entity, TCommand command)
    {
        // Default implementation does nothing.
    }
    
    /// <summary>
    /// Calculates the list of dates for which schedule items should be created.
    /// Can be overridden if the calculation logic differs significantly.
    /// </summary>
    protected virtual List<DateTime> CalculateScheduleDates(TCommand command)
    {
        var baseDate = GetBaseScheduleDate(command);
        if (!GetIsRecurring(command))
        {
            return new List<DateTime> { baseDate };
        }

        var parameters = GetRecurringParameters(command);
        
        var weekOfMonth = parameters.Length > 0 && parameters[0] is int w ? w : 0;
        
        return DateUtils.CalculateMonthlyRecurringSchedule(baseDate, weekOfMonth);
    }
    
    public async Task<Result> CreateScheduleAsync(TCommand command, TParentId parentId)
    {
        List<DateTime> scheduleDates;
        try
        {
            scheduleDates = CalculateScheduleDates(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating schedule dates.");
            // Provide a more specific error if possible
            return Result.Fail(new ApplicationError("Failed to calculate schedule dates.").CausedBy(ex));
        }


        if (scheduleDates == null || !scheduleDates.Any())
        {
            _logger.LogWarning("No schedule dates were generated for command associated with ParentId: {ParentId}", parentId);
            return Result.Ok().WithSuccess("No schedule dates generated based on input.");
        }

        _logger.LogInformation("Attempting to create {Count} schedule items for ParentId: {ParentId}", scheduleDates.Count, parentId);

        foreach (var scheduleDate in scheduleDates)
        {
             _logger.LogDebug("Processing schedule date: {ScheduleDate} for ParentId: {ParentId}", scheduleDate, parentId);

            TEntity scheduleEntity;
            try
            {
                // 1. Map core properties first
                scheduleEntity = MapToEntity(command, parentId);
                // 2. Set the specific date property
                SetScheduleDateOnEntity(scheduleEntity, scheduleDate);
                // 3. Allow customization (e.g., set IsActive)
                CustomizeEntityBeforeSave(scheduleEntity, command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing entity for date {ScheduleDate} and ParentId {ParentId}.", scheduleDate, parentId);
                return Result.Fail(new ApplicationError($"Failed to prepare schedule information for date {scheduleDate:yyyy-MM-dd}.").CausedBy(ex));
            }

            // 4. Save the fully prepared entity
            var saveResult = await SaveScheduleItemAsync(scheduleEntity);
            if (saveResult.IsFailed)
            {
                // Add context about which date failed
                return saveResult.WithError($"Failed during save operation for schedule date {scheduleDate:yyyy-MM-dd}.");
            }
        }

        _logger.LogInformation("Successfully created {Count} schedule items for ParentId: {ParentId}", scheduleDates.Count, parentId);
        return Result.Ok();
    }

    // --- Common Save Logic ---

    private async Task<Result> SaveScheduleItemAsync(TEntity entity)
    {
        try
        {
            var newId = await _repository.CreateAsync(entity);
            
            if (newId <= 0)
            {
                _logger.LogError("Repository CreateAsync returned a non-positive ID ({NewId}), indicating save failure for Entity: {Entity}", newId, entity);
                return Result.Fail(new ApplicationError("Failed to save schedule item to the database, invalid ID returned."));
            }

            _logger.LogDebug("Successfully saved schedule entity. Assigned ID: {NewId}", newId);
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error saving schedule entity to database.");
            return Result.Fail(new ExceptionalError("Unexpected error occurred saving schedule entity.", e));
        }
    }
}
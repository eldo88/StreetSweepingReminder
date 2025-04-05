
let registerUser = {
  username: String,
  email: String,
  password: String
};


export async function registerUserAsync (username, email, password){
  try {
    const response = await fetch(
      "http://localhost:5010/api/Auth/register",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ username, email, password }),
      }
    );

    const data = await response.json();
    console.log(data);
  } catch (error) {
    console.log(error);
  }
}

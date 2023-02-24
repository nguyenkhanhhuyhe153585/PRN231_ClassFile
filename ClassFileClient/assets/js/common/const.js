export let domain = "http://localhost:47858/";

export let Path = {
    Index : "/",
    Login : "/auth/login.html"
}

export let PathRight = {
    Anonymous : [Path.Login],
    Student : [],
    Teacher : [],
}

export let HttpCode = {
    Ok : 200,
    BadRequest : 400,
    UnAuthorized : 401,
}
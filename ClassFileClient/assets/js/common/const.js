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

export let BackEndApi = {
    Login : domain + "api/auth/login",
    ClassesHome : domain + "api/class",
}

export let HttpCode = {
    Ok : 200,
    BadRequest : 400,
    UnAuthorized : 401,
}

export let HttpMethod = {
    GET : "GET",
    POST : "POST",
    PUT : "PUT",
    DELETE : "DELETE"
}

export let HttpDataType = {
    JSON : "json",
    ApplicationJSON : "application/json"
}

export let Message = {
    Success : "Successful",
    Error : "",
    Oops: "Oops...",
    Process: "Processing",
}
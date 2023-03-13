export let DOMAIN = "http://localhost:47858/";

export const Path = {
  Index: "/",
  Login: "/auth/login.html",
  Signup: "/auth/signup.html",
  Class: "/class/index.html",
  Post: {
    Create: "/post/create.html",
    Edit: "/post/edit.html",
  },
  User: {
    Edit: "/user/myprofile/edit.html",
  },
};

export const PathRight = {
  Anonymous: [Path.Login, Path.Signup],
  Student: [],
  Teacher: [],
};

export const BackEndApi = {
  Login: DOMAIN + "api/auth/login",
  Signup: DOMAIN + "api/auth/signup",
  ClassesHome: DOMAIN + "api/class",
  Post: DOMAIN + "api/post",
  File: {
    Common: DOMAIN + "api/file",
    Attack: DOMAIN + "api/file",
    AvatarImage: DOMAIN + "api/file/avatar",
  },
  Account: {
    My: DOMAIN + "api/account/my",
    Edit: DOMAIN + "api/account/my/edit",
    EditPassword: DOMAIN + "api/account/my/edit/password",
  },
};

export const HttpCode = {
  Ok: 200,
  BadRequest: 400,
  UnAuthorized: 401,
};

export const HttpMethod = {
  GET: "GET",
  POST: "POST",
  PUT: "PUT",
  DELETE: "DELETE",
};

export const HttpDataType = {
  JSON: "json",
  ApplicationJSON: "application/json",
};

export const Message = {
  Success: "Successful",
  Error: "",
  Oops: "Oops...",
  Process: "Processing",
  FileTooLarge: "File size can not over 20MB.",
  NoFileSelect: "There is no file was selected!"
};

export const FileMode = {
  POST: "post",
  AVATAR: "avatar",
  CLASS: "class",
};

export const IMAGE_HOLDER = "https://via.placeholder.com/300x150";

export const PAGE = "page";
export const TOKEN = "token";
export const ID_PARAM = "id";
export const EMPTY_STRING = "";
export const MaxFileSize = 20971520;
export const imageHolder =
  "https://media.istockphoto.com/id/1226328537/vector/image-place-holder-with-a-gray-camera-icon.jpg?s=612x612&w=0&k=20&c=qRydgCNlE44OUSSoz5XadsH7WCkU59-l-dwrvZzhXsI=";

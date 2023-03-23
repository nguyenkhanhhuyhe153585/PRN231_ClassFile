export let DOMAIN = "http://localhost:47858/";

export const Path = {
  Index: "/",
  Login: "/auth/login.html",
  Signup: "/auth/signup.html",
  Class: {
    Create: "/class/create.html",
    Index: "/class/index.html",
    Member: "/class/member.html"
  },
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
  Classes: {
    Home: DOMAIN + "api/class",
    Create: DOMAIN + "api/class/create",
    Join: DOMAIN + "api/class/join",
    Leave: DOMAIN + "api/class/leave",
    Member: {
      Teacher: DOMAIN + "api/class/member/teacher",
      Student: DOMAIN + "api/class/member/student",
      TeacherOfClass: DOMAIN + "api/class/member"
    },
    Delete: DOMAIN + "api/class/delete",
    Regen: DOMAIN + "api/class/regen"
    
  },
  Post: DOMAIN + "api/post",
  File: {
    Index: DOMAIN + "api/file",
    Attack: DOMAIN + "api/file"
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
  Forbiden: 403,
  NotFound: 404
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

export const Role = {
  Teacher : "TC",
  Student: "STD",
}

export const Message = {
  Success: "Successful",
  Error: "",
  Oops: "Oops...",
  Process: "Processing",
  FileTooLarge: "File size can not over 20MB.",
  NoFileSelect: "There is no file was selected!",
};

export const FileMode = {
  POST: "post",
  AVATAR: "avatar",
  CLASS: "class",
};

export const IMAGE_HOLDER = "https://placehold.co/600x400/dddddd/dddddd?text=image";

export const Payload = {
  Typ: "typ",
  Name: "name"
}
export const PAGE = "page";
export const TOKEN = "token";
export const ID_PARAM = "id";
export const EMPTY_STRING = "";
export const MaxFileSize = 20971520;
export const imageHolder = IMAGE_HOLDER;

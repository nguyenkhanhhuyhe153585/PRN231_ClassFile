import * as Route from "./common/routing.js";
import * as Const from "./common/const.js";
import { ajaxEvent }  from "./common/config.js";
import { login } from "./component/auth/login.js";
import { loadClass } from "./component/index.js";
import { signup } from "./component/auth/signup.js";
import { initClassInfo, loadPostInClass } from "./component/class/class.js";

function main() {
  Route.includeHTML();
  Route.addAuthHeader();
  ajaxEvent();

  if (!Route.verifyAuth()) {
    return;
  }

  // Đăng kí các function với URL html
  if (Route.checkPath(Const.Path.Login)) {
    login();
  } else if (Route.checkPath(Const.Path.Index)) {
    loadClass();
  } else if (Route.checkPath(Const.Path.Signup)) {
    signup();
  } else if(Route.checkPath(Const.Path.Class)){
    initClassInfo();
    loadPostInClass();
  }
}


$(document).ready(main);

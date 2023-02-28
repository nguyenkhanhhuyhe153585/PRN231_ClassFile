import * as Route from "./common/routing.js";
import * as Const from "./common/const.js";
import { ajaxEvent }  from "./common/config.js";
import { login } from "./component/auth/login.js";
import { loadClass } from "./component/index.js";

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
  }
}


$(document).ready(main);

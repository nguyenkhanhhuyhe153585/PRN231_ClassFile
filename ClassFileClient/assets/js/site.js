import * as Route from "./common/routing.js";
import * as Const from "./common/const.js";
import { login } from "./auth/login.js";
import { loadInfo } from "./index.js";

function main() {
  Route.includeHTML();
  if (Route.verifyAuth()) {
    return;
  }

  if (Route.checkPath(Const.Path.Login)) {
    login();
  } else if (Route.checkPath(Const.Path.Index)) {
    loadInfo();
  }
}

$(document).ajaxStart(Route.addAuthHeader);
$(document).ajaxSend(function () {});
$(document).ajaxError(function (event, xhr, settings) {
  let message = xhr.responseJSON?.message
  Swal.fire({
    icon: "error",
    title: "Oops...",
    text: message,
    footer: xhr.status + " - " + xhr.statusText,
  });
});
$(document).ajaxComplete(function (event, xhr, settings) {
  if (xhr.status === Const.HttpCode.UnAuthorized) {
    Route.redirect(Const.Path.Login);
  } else if (xhr.status === Const.HttpCode.Ok) {
    Swal.fire({
      position: "top-end",
      icon: "success",
      title: "Your work has been saved",
      showConfirmButton: false,
      timer: 1500,
    });
  }
});
$(document).ready(main);

import { setCookie } from "../common/cookies.js";
import * as CONST from "../common/const.js";
import * as Route from "../common/routing.js"

export function login() {
  $("#formLogin").submit(function (event) {
    event.preventDefault();
    postLogin(); 
  });

  function postLogin() {
    let userName = $("#inputEmail").val();
    let password = $("#inputPassword").val();

    let option = {};
    option.url = CONST.domain + "api/auth/login";
    option.method = "POST";
    option.contentType = "application/json";
    option.dataType = "json";
    option.data = JSON.stringify({
      username: userName,
      password: password,
    });
    option.complete = function(xhr){
      let data = xhr.responseJSON;
      if(xhr.status === CONST.HttpCode.Ok){
        setCookie("token", data.token, 1);
        Route.redirect(CONST.Path.Index);
      }
      else if(xhr.status === CONST.HttpCode.UnAuthorized){
        $("#textLoginError").html(data.message);
      }
    };
    
    $.ajax(option);
  }
}

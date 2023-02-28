import * as CONST from "../../common/const.js";
import * as Route from "../../common/routing.js"

export function signup() {
    $('#formSignup').submit(function (event) {
        event.preventDefault();
        postSignup();
    });

    function postSignup() {
        let fullName = $("#inputFullname").val();
        let userName = $("#inputUsername").val();
        let password = $("#inputPassword").val();
        let password2 = $("#inputPassword2").val();
        let accountType = $("#selectAccType").val();

        let opt = {};

        opt.url = CONST.BackEndApi.Signup;
        opt.method = CONST.HttpMethod.POST;
        opt.contentType = CONST.HttpDataType.ApplicationJSON;
        opt.dataTpe = CONST.HttpDataType.JSON;
        opt.data = JSON.stringify({
            fullname: fullName,
            username: userName,
            password: password,
            password2: password2,
            accounttype: accountType
        });
        opt.complete = function(xhr) {
            let data = xhr.responseJSON;
            if (xhr.status === CONST.HttpCode.Ok) {
                Route.redirect(CONST.Path.Login);
            }
            else if(xhr.status === CONST.HttpCode.UnAuthorized){
                $("#textLoginError").html(data.message);
            }
        };

        $.ajax(opt);
    }
}
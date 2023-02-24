import * as Const from "../js/common/const.js";

export function loadInfo(){
    $("#loadInfo").click(function(){
        let option = {};
        option.url = Const.domain + "api/auth/info";
        option.type = "POST",
        option.dataType = "json";
        option.success = function (data){
            console.log(data);
        }
        
        $.ajax(option);
    })
}
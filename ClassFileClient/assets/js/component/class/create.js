import * as Const from "../../common/const.js";
import * as Cookies from "../../common/cookies.js";
import * as Utils from "../../common/utils.js";

export function createClass() {
    $("#formCreateClass").submit(function(event) {
        event.preventDefault();

        let token = Cookies.getCookie("token");
        let payloadData = Utils.parseJwt(token);
        let teacherId = payloadData['name'];
        let teacherType = payloadData['role'];
        let className = $("#inputClassName").val();
        // let classCode = $("#inputClassCode").val();
    
        let opt = {};
        opt.url = Const.BackEndApi.Classes.Create;
        opt.method = Const.HttpMethod.POST;
        opt.contentType = Const.HttpDataType.ApplicationJSON;
        opt.dataType = Const.HttpDataType.JSON;
        opt.data = JSON.stringify({
            classname: className,
            accountprofile: {
                id: teacherId,
                accounttype: teacherType
            }
        });
        // opt.success = function (response) {
        //     history.back();
        // };
        
        $.ajax(opt);
    });
    
}


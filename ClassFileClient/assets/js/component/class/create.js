import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";

export function createClass() {
    let token = Cookies.getCookie("token");
    let payloadData = Utils.parseJwt(token);
    let teacherId = payloadData['name'];
    let teacherType = payloadData['role'];
    let className = $("#inputClassName").val();
    let classCode = $("#inputClassCode").val();

    let opt = {};
    opt.url = Const.BackEndApi.ClassesHome;
    opt.method = Const.HttpMethod.POST;
    opt.contentType = Const.HttpDataType.ApplicationJSON;
    opt.dataType = Const.HttpDataType.JSON;
    opt.data = JSON.stringify({
        classname: className,
        classcode: classCode,
        teacher: {
            id: teacherId,
            accounttype: teacherType
        }
    });
    opt.complete = function(xhr) {
        let data = xhr.responseJSON;
        
    }
    
}


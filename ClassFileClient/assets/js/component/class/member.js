import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";
import * as Utils from "../../common/utils.js";
import * as Cookies from "../../common/cookies.js";

export function classMember() {
    distinguishRoles();
}

function distinguishRoles() {
    let token = Cookies.getCookie("token");
    let payloadData = Utils.parseJwt(token);
    let type = payloadData["typ"];
    let id = Route.getUrlParam("id");
    
    let option = {};
    option.url = Const.BackEndApi.Classes.Member.TeacherOfClass + `/${id}`;
    option.type = Const.HttpMethod.GET;
    option.dataType = Const.HttpDataType.JSON;
    option.success = function (data) {
        if (type === Const.Role.Teacher) {
            loadListStudentForTeacher(data.fullname);
        } else if (type === Const.Role.Student) {
            loadListStudentForStudent(data.fullname);
        }
    };
    
    $.ajax(option);
}

function loadListStudentForTeacher(teacherName) {
    let classId = Route.getUrlParam("id");
    let option = {};
    option.url = Const.BackEndApi.Classes.Member.Teacher + `/${classId}`;
    option.type = Const.HttpMethod.GET;
    option.dataType = Const.HttpDataType.JSON;
    option.success = function (data) {
        render(data);
    };

    $.ajax(option);

    function render(data) {
        console.log(data);
        let profileTeacher = 
        `<div class="row mb-4">
            <table>
                <thead class="py-4">
                    <tr>
                        <th>
                            <span class="h1">Teacher</span>
                        </th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr style="border-top: 2px solid;">
                        <td style="width: 40px;" class="pt-2">
                            <img id="accountAvatar" src="https://s3.eu-central-1.amazonaws.com/bootstrapbaymisc/blog/24_days_bootstrap/fox.jpg"
                            width="40" height="40" class="rounded-circle nav-item border me-1">
                        </td>
                        <td>
                            <span class="h6" id="teacherName">
                                ${teacherName}
                            </span>
                        </td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
        </div>`;
        let profileStudents = 
        `<div class="row">
            <table>
                <thead>
                    <tr style="border-bottom: 2px solid;">
                        <th>
                            <span class="h1">Students</span>
                        </th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody id="r1"></tbody>
            </table>
        </div>`;
        $("#classMember").html(profileTeacher);
        $("#classMember").append(profileStudents);
        for (let e of data) {
            $("#r1").append(
                `<tr style="border-top: 1px solid";>
                    <td style="width: 40px;" class="py-2">
                        <img id="accAva" src="https://s3.eu-central-1.amazonaws.com/bootstrapbaymisc/blog/24_days_bootstrap/fox.jpg"
                        width="40" height="40" class="rounded-circle nav-item border me-1">
                    </td>
                    <td class="py-2">
                        <span class="h6">
                            ${e.fullname}
                        </span>
                    </td>
                    <td class="d-flex justify-content-end py-2">
                        <button class="deleteStudent" data-studentId="${e.id}">
                            <i class="fa-solid fa-user-minus"></i>
                        </button>
                    </td>
                </tr>`);
        }

        $(".deleteStudent").click(function (){
            let studentId = $(this).attr("data-studentId");
            let opt = {};
            opt.url = Const.BackEndApi.Classes.Member.Teacher + `/${classId}`;
            opt.method = Const.HttpMethod.DELETE;
            opt.contentType = Const.HttpDataType.ApplicationJSON;
            opt.dataType = Const.HttpDataType.JSON;
            opt.data = JSON.stringify({
                id: studentId
            });
            opt.success = function() {
                Swal.fire({
                    icon: "success",
                    title: Const.Message.Success,
                    showConfirmButton: false,
                    timer: 1500,
                  }).then(Route.reload);
            }
            $.ajax(opt);
            console.log("da xoa" + studentId);
        });
    }
}

function loadListStudentForStudent(teacherName) {
    let classId = Route.getUrlParam("id");
    let option = {};
    option.url = Const.BackEndApi.Classes.Member.Student + `/${classId}`;
    option.type = Const.HttpMethod.GET;
    option.dataType = Const.HttpDataType.JSON;
    option.success = function (data) {
        render(data);
    };

    $.ajax(option);

    function render(data) {
        console.log(data);
        let profileTeacher = 
        `<div class="row mb-4">
            <table>
                <thead class="py-4">
                    <tr>
                        <th>
                            <span class="h1">Teacher</span>
                        </th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr style="border-top: 2px solid;">
                        <td style="width: 40px;" class="pt-2">
                            <img id="accountAvatar" src="https://s3.eu-central-1.amazonaws.com/bootstrapbaymisc/blog/24_days_bootstrap/fox.jpg"
                            width="40" height="40" class="rounded-circle nav-item border me-1">
                        </td>
                        <td>
                            <span class="h6" id="teacherName">
                                ${teacherName}
                            </span>
                        </td>
                        <td></td>
                    </tr>
                </tbody>
            </table>
        </div>`;
        let profileStudents = 
        `<div class="row">
            <table>
                <thead>
                    <tr style="border-bottom: 2px solid;">
                        <th>
                            <span class="h1">Students</span>
                        </th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody id="r1"></tbody>
            </table>
        </div>`;
        $("#classMember").html(profileTeacher);
        $("#classMember").append(profileStudents);
        for (let e of data) {
            $("#r1").append(
                `<tr style="border-top: 1px solid";>
                    <td style="width: 40px;" class="py-2">
                        <img id="accAva" src="https://s3.eu-central-1.amazonaws.com/bootstrapbaymisc/blog/24_days_bootstrap/fox.jpg"
                        width="40" height="40" class="rounded-circle nav-item border me-1">
                    </td>
                    <td class="py-2">
                        <span class="h6">
                            ${e.fullname}
                        </span>
                    </td>
                    <td class="d-flex justify-content-end py-2">
                    </td>
                </tr>`);
        }
    }
}
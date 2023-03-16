import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";
// import * as Utils from "../../common/utils.js";
// import * as Cookies from "../../common/cookies.js";

export function classProfile() {
    loadListStudent();
}

function loadListStudent() {
    let classId = Route.getUrlParam("id");
    let option = {};
    option.url = Const.BackEndApi.Classes.Profile + `/${classId}`;
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
                <thead>
                    <tr>
                        <th>
                            <span class="h1">Teacher</span>
                        </th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody class="mt-2 d-lg-table-cell">
                    <tr style="border-top: 1px solid;">
                        <td style="width: 40px;">
                            <img id="accountAvatar" src="https://s3.eu-central-1.amazonaws.com/bootstrapbaymisc/blog/24_days_bootstrap/fox.jpg"
                            width="40" height="40" class="rounded-circle nav-item border me-1">
                        </td>
                        <td>
                            <span class="h6" id="teacherName">
                                ${data[0].fullname}
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
                    <tr style="border-bottom: 1px solid;">
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
        $("#classProfile").html(profileTeacher);
        $("#classProfile").append(profileStudents);
        for (let index = 1; index < data.length; index++) {
            const element = data[index];
            $("#r1").append(
                `<tr>
                    <td style="width: 40px;">
                        <img id="accAva" src="https://s3.eu-central-1.amazonaws.com/bootstrapbaymisc/blog/24_days_bootstrap/fox.jpg"
                        width="40" height="40" class="rounded-circle nav-item border me-1">
                    </td>
                    <td>
                        <span class="h6">
                            ${element.fullname}
                        </span>
                    </td>
                    <td class="d-flex justify-content-end">
                        <img id="deleteStudent" src="https://cdn-icons-png.flaticon.com/512/10015/10015394.png"
                        width="40" height="40" class="rounded-circle nav-item border ms-auto">
                    </td>
                </tr>`);
            $("#deleteStudent").click(function (){
                console.log("da xoa");
                // let opt = {};
                // opt.url = Const.BackEndApi.Classes.Create;
                // opt.method = Const.HttpMethod.POST;
                // opt.contentType = Const.HttpDataType.ApplicationJSON;
                // opt.dataType = Const.HttpDataType.JSON;
                // opt.data = JSON.stringify({
                // classname: className,
                // accountprofile: {
                //     id: teacherId,
                //     accounttype: teacherType,
                // },
                // imageCover: fileImageResponseName
                // });
                // $.ajax(opt);
            });
        }
    }

    
}
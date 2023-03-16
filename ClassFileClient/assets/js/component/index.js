import * as Const from "../common/const.js";
import * as Utils from "../common/utils.js";

export function index() {
  loadClass();
  initAction();
}

function initAction() {
  if (Utils.checkRole(Const.Role.Teacher)) {
    $("#actionButton").append(
      `<a class="btn btn-primary my-2" href="/class/create.html"><i class="fa-solid fa-plus me-2"></i>Create a class</a>`
    );
  } else if (Utils.checkRole(Const.Role.Student)) {
    $("#actionButton").append(
      `<button class="btn btn-primary my-2" id="buttonPopupJoinClass"><i class="fa-solid fa-plus me-2"></i>Join a class</button>`
    );
    joinClass();
  }
}

function joinClass() {
  $("#buttonPopupJoinClass").click(function () {
    Swal.fire({
      title: "Provide class code",
      input: "text",
      inputAttributes: {
        autocapitalize: "off",
      },
      showCancelButton: true,
      confirmButtonText: "Join",
      showLoaderOnConfirm: true,
      preConfirm: function (classCode) {
        $.ajax({
          url: Const.BackEndApi.Classes.Join,
          type: Const.HttpMethod.POST,
          contentType: Const.HttpDataType.ApplicationJSON,
          dataType: Const.HttpDataType.JSON,
          data: JSON.stringify({
            classCode: classCode,
          }),
        });
      },

      // allowOutsideClick: () => !Swal.isLoading(),
    });
  });
}

function loadClass() {
  let option = {};
  option.url = Const.BackEndApi.Classes.Home;
  (option.type = Const.HttpMethod.GET),
    (option.dataType = Const.HttpDataType.JSON);
  option.success = function (data) {
    render(data);
    console.log(data);
  };

  $.ajax(option);

  function render(data) {
    let result = "";
    for (let c of data) {
      result += `
            <div class="col">
                <div class="card">
                <div>
                <div class="card-img-overlay">
                        <img class="rounded-circle border img-avatar d-inline-block" src="${Utils.getUrlImage(
                          Const.FileMode.AVATAR,
                          c.teacherAccount.imageAvatar
                        )}" />                        
                    </div>
                    <img src="${Utils.getUrlImage(
                      Const.FileMode.CLASS,
                      c.imageCover
                    )}" class="img-cover-card card-img-top" alt="class cover">
                    </div>
                    <div class="card-body">
                        <h5 class="card-title">${c.className}</h5>
                        <p class="card-text">${c.teacherAccount.fullname}</p>
                        <a class="stretched-link" href="${
                          Const.Path.Class.Index
                        }?id=${c.id}"></a>
                    </div>
                    <div class="card-footer">
                        <small class="text-muted">Last updated: ${Utils.formatDate(
                          c.lastPost
                        )}</small>
                    </div>
                </div>
            </div>
            `;
    }
    $("#classesList").html(result);
  }
}

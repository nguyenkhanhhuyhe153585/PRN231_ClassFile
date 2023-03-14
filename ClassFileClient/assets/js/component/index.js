import * as Const from "../common/const.js";
import * as Utils from "../common/utils.js";

export function index() {
  loadClass();
  joinClass();
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
      confirmButtonText: "Look up",
      showLoaderOnConfirm: true,
      preConfirm: async (login) => {
        try {
          const response = await fetch(`//api.github.com/users/${login}`);
          if (!response.ok) {
            throw new Error(response.statusText);
          }
          return await response.json();
        } catch (error) {
          Swal.showValidationMessage(`Request failed: ${error}`);
        }
      },

      allowOutsideClick: () => !Swal.isLoading(),
    }).then((result) => {
      if (result.isConfirmed) {
        Swal.fire({
          title: `${result.value.login}'s avatar`,
          imageUrl: result.value.avatar_url,
        });
      }
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
                <div class="card h-100">
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
                    )}" class="card-img-top" alt="...">
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

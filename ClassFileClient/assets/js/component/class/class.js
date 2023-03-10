import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";
import * as Utils from "../../common/utils.js";
import * as Cookies from "../../common/cookies.js";

export function classAction() {
  initClassInfo();
  loadPostInClass();
}

function initClassInfo() {
  let classId = Route.getUrlParam("id");
  let option = {};
  option.url = Const.BackEndApi.Classes.Home + `/${classId}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
  };

  $.ajax(option);

  function render(data) {
    console.log(data);
    $("#createPostButton").attr(
      "href",
      `${Const.Path.Post.Create}?classId=${data.id}`
    );
    $("#classCoverImage").attr("src", Utils.getUrlImage(Const.FileMode.CLASS, data.imageCover));
    $("#classNameCover").html(data.className);
    $("head title", window.parent.document).text(data.className);
    initPanel(data);
  }
}

function initPanel(data){
  if(Route.checkRole(Const.Role.Teacher)){
    let result = `
    <div class="row mb-3">
        <div class="col">
            <div class="card">
                <div class="card-body">
                    <h6 class="card-subtitle mb-2">Class Code:</h6>
                    <h5 class="card-title mb-2" id="classCode">${data.classCode}</h5>
                    <a href="#" class="card-link mb-2">RegenCode</a>
                    <p class="card-text mt-3 text-muted">(Provide this code for student can join this
                        class)
                    </p>
                </div>
            </div>
        </div>
    </div>
    `;
    $("#actionPanel").append(result);
  }
}

/**
 * Promise Object, sẽ thực thi các callback function khi resolse được gọi tới
 */
function loadPostInClass() {
  let classId = Route.getUrlParam("id");
  let page = Route.getUrlParam("page");
  page = Number(page);
  let option = {};
  option.url = Const.BackEndApi.Post + `?classId=${classId}&page=${page}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
    // thực hiện render paging
    Utils.pagination(data);
  };

  $.ajax(option);

  function render(data) {
    const currentUserId = Utils.parseJwt(Cookies.getCookie(Const.TOKEN)).name;
    let dataPosts = data.data;
    let result = "";
    for (let post of dataPosts) {
      result += `
        <div class="row mb-3">
            <div class="col">
                <div class="card">
                    <div class="card-header">
                        <img src="${Utils.getUrlImage(Const.FileMode.AVATAR, post.postedAccount.imageAvatar)}"
                            width="40" height="40" class="rounded-circle nav-item border me-1">
                        <span class="h6">
                            ${post.postedAccount.fullname}
                        </span>
                        <span>|</span>
                        <span class="">
                            ${Utils.formatDate(post.dateCreated )}
                        </span>
                        <div class="dropdown d-inline ms-3">
                            <button class="btn btn-secondary btn-sm dropdown-toggle" type="button"
                                id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false">
                                Option
                            </button>
                            <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1">
                            ${(function () {
                              if (post.postedAccount.id == currentUserId) {
                                return `
                                <li><a class="dropdown-item" href="${Const.Path.Post.Edit}?id=${post.id}">Edit</a></li>
                                `;
                              }
                            })()}
                                <li><a class="dropdown-item" href="#">Another action</a></li>
                                <li><a class="dropdown-item" href="#">Something else here</a></li>
                            </ul>
                        </div>
                    </div>
                    <div class="card-body">
                        <p class="card-text">
                          ${post.title}
                        </p>
                        ${(function () {
                          let resultLinkFile = "";
                          for (let file of post.files) {
                            resultLinkFile += `
                            <a href="${Const.BackEndApi.File.Index}/${file.fileName}" 
                            class="d-inline-block link-primary me-3" download="${file.fileNameRoot}">
                            ${file.fileNameRoot}
                            </a>`;
                          }
                          return resultLinkFile;
                        })()}                  
                    </div>
                </div>
            </div>
        </div>
    `;
    }
    $("#postContainter").html(result);
  }
}

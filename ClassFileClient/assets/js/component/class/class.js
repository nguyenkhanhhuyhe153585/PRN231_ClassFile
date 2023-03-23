import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";
import * as Utils from "../../common/utils.js";
// import * as Cookies from "../../common/cookies.js";

export function classAction() {
  initClassInfo();
  loadPostInClass();
  deletePost();
  
}

function classMenu(data) {
  let result = `<li><a class="dropdown-item" href="member.html?id=${data.id}">Members</a></li>`;
  if (Utils.checkRole(Const.Role.Teacher) && Utils.checkUser(data.teacherAccount.id)) {
    result += `
        <li><a class="dropdown-item" href="edit.html?id=${data.id}">Edit</a></li>
        <li><a class="dropdown-item deleteClass" href="javascript:void(0)" data-classId="${data.id}">Delete Class</a></li>
    `;
  } else if(Utils.checkRole(Const.Role.Student)){
   result += `<li><a class="dropdown-item leaveClass" data-classId="${data.id}">Leave Class</a></li>`;
  }

  $("#classMenu").html(`
    <button class="btn btn-secondary btn-sm dropdown-toggle" type="button"
        id="dropdownMenuButtonClass" data-bs-toggle="dropdown" aria-expanded="false">
        Option
    </button>
    <ul class="dropdown-menu" aria-labelledby="dropdownMenuButtonClass">
        ${result}
    </ul>
  `);
  deleteClass();
  leaveClass();
}

function leaveClass(){
  $("a.leaveClass").click(function () {
    let classId = $(this).attr("data-classId");
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, leave it!",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: Const.BackEndApi.Classes.Leave + `/${classId}`,
          type: Const.HttpMethod.POST,
          contenType: Const.HttpDataType.ApplicationJSON,
          success: function(){},
        });
      }
    });
  });
}

function deleteClass() {
  $("a.deleteClass").click(function () {
    let classId = $(this).attr("data-classId");
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: Const.BackEndApi.Classes.Delete + `/${classId}`,
          type: Const.HttpMethod.DELETE,
        });
      }
    });
  });
}

function deletePost() {
  $("a.deletePost").click(function () {
    let postId = $(this).attr("data-postId");
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: Const.BackEndApi.Post + `/${postId}`,
          type: Const.HttpMethod.DELETE,
          suppressGlobalComplete: true,
          success: function() {
            Swal.fire({
              icon: "success",
              title: Const.Message.Success,
              showConfirmButton: false,
              timer: 1500,
            }).then(Route.reload);
          }
        });
      }
    });
  });
}

function initClassInfo() {
  let classId = Route.getUrlParam("id");
  let option = {};
  option.url = Const.BackEndApi.Classes.Home + `/${classId}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
    // thực hiện init class menu
    classMenu(data);
  };

  $.ajax(option);

  function render(data) {
    console.log(data);
    $("#createPostButton").attr(
      "href",
      `${Const.Path.Post.Create}?classId=${data.id}`
    );
    $("#classCoverImage").attr(
      "src",
      Utils.getUrlImage(Const.FileMode.CLASS, data.imageCover)
    );
    $("#classNameCover").html(data.className);
    $("head title", window.parent.document).text(data.className);
    initPanel(data);
  }
}

function initPanel(data) {
  let result = "";
  if (Route.checkRole(Const.Role.Teacher)) {
    result += `
    <div class="row mb-3">
        <div class="col">
            <div class="card">
                <div class="card-body">
                    <h6 class="card-subtitle mb-2">Class Code:</h6>
                    <h5 class="card-title mb-2" id="classCode">${data.classCode}</h5>
                    <a href="javascript:void(0)" class="card-link mb-2 regen" data-classId=${data.id}>RegenCode</a>
                    <p class="card-text mt-3 text-muted">(Provide this code for student can join this
                        class)
                    </p>
                </div>
            </div>
        </div>
    </div>
    `;
  }
  result += `
    <div class="row mb-3">
        <div class="col">
            <div class="card">
                <div class="card-body">
                    <h6 class="card-title mb-2">Class Rule:</h6>
                    <p class="card-text mt-3 text-muted">You can create a post, upload and download files in here.               
                    </p>
                    <p class="card-text mt-3 text-muted">Everyone in this class can see your post and download attachments
                    </p>
                    <p class="card-text mt-3 text-muted">(Teacher has all right to kick out any class member)
                    </p>
                </div>
            </div>
        </div>
    </div>
    `;

  $("#actionPanel").html(result);
  regenCode();
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
    // thực hiện init delete Post;
    deletePost();
    
  };

  $.ajax(option);

  function render(data) {
    let dataPosts = data.data;
    let result = "";
    for (let post of dataPosts) {
      result += `
          <div class="row mb-3">
              <div class="col">
                  <div class="card">
                      <div class="card-header">
                          <img src="${Utils.getUrlImage(
                            Const.FileMode.AVATAR,
                            post.postedAccount.imageAvatar
                          )}"
                              class="img-avatar-post rounded-circle nav-item border me-1">
                          <span class="h6">
                              ${post.postedAccount.fullname}
                          </span>
                          <span>|</span>
                          <span class="">
                              ${Utils.formatDate(post.dateCreated)}
                          </span>
                          
                              ${(function () {
                                if (Utils.checkUser(post.postedAccount.id)) {
                                  return `
                                  <div class="dropdown d-inline ms-3">
                              <button class="btn btn-secondary btn-sm dropdown-toggle" type="button"
                                  id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false">
                                  Option
                              </button>
                              <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1">
                                  <li><a class="dropdown-item" href="${Const.Path.Post.Edit}?id=${post.id}">Edit</a></li>
                                  <li><a class="dropdown-item deletePost" href="javascript:void(0)" data-postId=${post.id}>Delete</a></li>
                              </ul>
                              </div>
                                  `;
                                }
                                return "";
                              })()}
                          
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

function regenCode() {
  $(".regen").click(function () {
    let classId = $(this).attr("data-classId");

    let option = {};
    option.url = Const.BackEndApi.Classes.Regen + `/${classId}`;
    option.type = Const.HttpMethod.PUT;
    option.dataType = Const.HttpDataType.JSON;
    option.suppressGlobalComplete = true;
    option.success = function() {
      Swal.fire({
        icon: "success",
        title: Const.Message.Success,
        showConfirmButton: false,
        timer: 1500,
      }).then(Route.reload);
    }
  
    $.ajax(option);
  })
}


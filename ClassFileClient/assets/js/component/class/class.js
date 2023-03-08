import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";

export function initClassInfo() {
  let classId = Route.getUrlParam("id");

  let option = {};
  option.url = Const.BackEndApi.ClassesHome + `/${classId}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
  };

  $.ajax(option);

  function render(data) {
    $("#createPostButton").attr("href", `${Const.Path.Post.Create}?classId=${data.id}`)
    $("#classNameCover").html(data.className);
    $("head title", window.parent.document).text(data.className);
  }
}

export function loadPostInClass() {
  let classId = Route.getUrlParam("id");

  let option = {};
  option.url = Const.BackEndApi.Post + `?classId=${classId}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (data) {
    render(data);
  };

  $.ajax(option);

  function render(data) {
    let result = "";
    for (let post of data) {
      result += `
      <div class="row mb-3">
          <div class="col-lg-8">
              <div class="card">
                  <div class="card-header">
                      <img src="https://s3.eu-central-1.amazonaws.com/bootstrapbaymisc/blog/24_days_bootstrap/fox.jpg"
                          width="40" height="40" class="rounded-circle nav-item border me-1">
                      <span class="h6">
                          ${post.postedAccount.fullname}
                      </span>
                      <span>|</span>
                      <span class="">
                          ${post.dateCreated}
                      </span>
                      <div class="dropdown d-inline ms-3">
                          <button class="btn btn-secondary btn-sm dropdown-toggle" type="button"
                              id="dropdownMenuButton1" data-bs-toggle="dropdown" aria-expanded="false">
                              Option
                          </button>
                          <ul class="dropdown-menu" aria-labelledby="dropdownMenuButton1">
                              <li><a class="dropdown-item" href="${Const.Path.Post.Edit}?id=${post.id}">Edit</a></li>
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
                          <a href="${Const.BackEndApi.File}/${file.fileName}" 
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

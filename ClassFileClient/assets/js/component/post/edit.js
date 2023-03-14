import * as Const from "../../common/const.js";
import * as Route from "../../common/routing.js";
import * as Utils from "../../common/utils.js";

export function editPost() {
  initEditPostData();
  doEditPost();
}

async function initEditPostData() {
  const postId = Route.getUrlParam(Const.ID_PARAM);
  // Add as attribute for form element;
  $("#formCreatePost").attr("data-postId", postId);

  let option = {};
  option.url = Const.BackEndApi.Post + `/${postId}`;
  option.type = Const.HttpMethod.GET;
  option.dataType = Const.HttpDataType.JSON;
  option.success = function (response) {
    render(response).then(initDeleteFile);
  };

  async function render(data) {
    $("#contentPostTextArea").val(data.title);
    console.log(data);
    let result = "";
    for (let f of data.files) {
      result += `
            <tr>
                <td class="fileName"><a href="${Const.BackEndApi.File}/${f.fileName}" target="_blank" data-fileId="${f.id}">${f.fileNameRoot}</a></td>
                <td class="text-end btnRow"><a href="#" class="btnRemoveFile text-danger">Remove</a></td>
            </tr>
            `;
    }
    $("#classNameTitle").html(data.class.className);
    $("#tableFilePreview").html(result);
    $("#imageAvatar").attr("src", Utils.getUrlImage(Const.FileMode.AVATAR, data.postedAccount.imageAvatar));
    $("#postedAccountName").html(data.postedAccount.fullname);
    $("#postedDatetime").html(Utils.formatDate(data.dateCreated));
  }

  $.ajax(option);
}

function initDeleteFile() {
  $("a.btnRemoveFile").click(function (event) {
    event.preventDefault();
    let fileId = $(this)
      .parent()
      .parent()
      .children(".fileName")
      .children()
      .attr("data-fileId");
    if (fileId) {
      console.log(fileId);
      deleteFile(fileId);
    }
  });
}

function deleteFile(fileId) {
  const postId = Route.getUrlParam(Const.ID_PARAM);

  let option = {};
  option.url = Const.BackEndApi.File.Index + `/${postId}/${fileId}`;
  option.type = Const.HttpMethod.DELETE;
  option.success = function(){
    initEditPostData();
  }
  $.ajax(option);
}

function doEditPost(){
  $("#formCreatePost").submit(function (event) {
    event.preventDefault();

    // Tạo form data
    let form_data = new FormData();

    let postId = $("#formCreatePost").attr("data-postId");
    let contentText = $("#contentPostTextArea").val();
    let file_datas = $("#fileUploadChoose").prop("files");

    form_data.append("id", postId);
    form_data.append("content", contentText);
    form_data.append("fileMode", Const.FileMode.POST);

    let totalFileSize = 0;

    for (let fdata of file_datas) {
      totalFileSize += fdata.size;
      
      form_data.append("file", fdata);
    }
    if(totalFileSize >= Const.MaxFileSize){
      let message = Const.Message.FileTooLarge;
      Swal.fire({
        icon: "error",
        title: Const.Message.Oops,
        text: message,
      });
      return;
    }

    let option = {};
    option.url = Const.BackEndApi.Post;
    option.data = form_data;
    option.type = Const.HttpMethod.PUT;
    option.processData = false;
    option.contentType = false;
    option.cache = false;
    $.ajax(option);
  });
}

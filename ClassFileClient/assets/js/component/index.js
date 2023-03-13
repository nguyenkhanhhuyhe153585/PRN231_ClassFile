import * as Const from "../common/const.js";
import * as Utils from "../common/utils.js";

export function loadClass(){
    
        let option = {};
        option.url = Const.BackEndApi.ClassesHome;
        option.type = Const.HttpMethod.GET,
        option.dataType = Const.HttpDataType.JSON;
        option.success = function (data){
            render(data);
            console.log(data);
        }
        
        $.ajax(option);  

        function render(data){
            let result = "";
            for(let c of data){
                result += 
                `
            <div class="col">
                <div class="card h-100">
                    <img src="https://via.placeholder.com/300x150" class="card-img-top" alt="...">
                    <div class="card-img-overlay">
                        <img class="rounded-circle border img-avatar d-inline-block" src="${
                            Utils.getUrlImage(c.teacherAccount.imageAvatar)
                        }" />                        
                    </div>
                    <div class="card-body">
                        <h5 class="card-title">${c.className}</h5>
                        <p class="card-text">${c.teacherAccount.fullname}</p>
                        <a class="stretched-link" href="${Const.Path.Class}?id=${c.id}"></a>
                    </div>
                    <div class="card-footer">
                        <small class="text-muted">Last updated ${c.lastPost}</small>
                    </div>
                </div>
            </div>
            `;
            }
            $("#classesList").html(result);
        }
}
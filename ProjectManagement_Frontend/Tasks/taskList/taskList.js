const baseUrl = 'https://localhost:7075/api';

let allProjects = [];
let allUsers = [];
let projectMembers = [];

// Sayfa yüklendiğinde projeleri, kullanıcıları ve TÜM görevleri otomatik getir
window.onload = async () => {
    loadUserDataToHeader();
    await loadProjectsForSearch();
    await loadUsersForSearch();
    await fetchTasks(); // Filtresiz, direkt tüm verileri çeker
};

function loadUserDataToHeader() {
    const firstName = localStorage.getItem('firstName') || "Kullanıcı";
    const lastName = localStorage.getItem('lastName') || "";
    const roleName = localStorage.getItem('roleName') || "Rol Belirtilmemiş";

    const userNameText = document.getElementById('userNameText');
    const userRoleText = document.getElementById('userRoleText');
    const userAvatarInitial = document.getElementById('userAvatarInitial');

    if (userNameText) userNameText.innerText = `${firstName} ${lastName}`;
    if (userRoleText) userRoleText.innerText = roleName;
    if (userAvatarInitial) userAvatarInitial.innerText = firstName ? firstName.charAt(0).toUpperCase() : "U";
}

async function loadProjectsForSearch() {
    const datalist = document.getElementById('projectOptions');
    const addProjectList = document.getElementById("addProjectOptions");

    try {
        const response = await fetch(`${baseUrl}/Projects?page=1&pageSize=1000`, {
            headers: { 'Authorization': 'Bearer ' + localStorage.getItem('authToken') }
        });
        
        if (response.ok) {

    const result = await response.json();
    allProjects = result.data ? result.data : result;

    if (datalist)
        datalist.innerHTML = "";

    if (addProjectList)
        addProjectList.innerHTML = "";

    allProjects.forEach(p => {

        if (datalist)
            datalist.innerHTML += `<option value="${p.name}">`;

        if (addProjectList)
            addProjectList.innerHTML += `<option value="${p.name}">`;

    });
}
    } catch (error) {
        console.error("Projeler yüklenirken hata:", error);
    }
}

async function loadUsersForSearch() {
    const datalist = document.getElementById('userOptions');
    const addUserList = document.getElementById("addUserOptions");

    try {
        const response = await fetch(`${baseUrl}/users?page=1&pageSize=1000`, {
            headers: { 'Authorization': 'Bearer ' + localStorage.getItem('authToken') }
        });
        
        if (response.ok) {

    const result = await response.json();
    allUsers = result.data ? result.data : result;

    if (datalist)
        datalist.innerHTML = "";

    if (addUserList)
        addUserList.innerHTML = "";

    allUsers.forEach(u => {

        const fullName = `${u.firstName} ${u.lastName}`;

        if (datalist)
            datalist.innerHTML += `<option value="${fullName}">`;

        if (addUserList)
            addUserList.innerHTML += `<option value="${fullName}">`;

    });
}
    } catch (error) {
        console.error("Kullanıcılar yüklenirken hata:", error);
    }
}

// Tüm Görevleri Getiren ve İsteğe Bağlı Filtreleyen Ana Fonksiyon
async function fetchTasks() {
    const params = new URLSearchParams();

    // Sayfalama kısıtlamasını kaldırıp tüm verileri çekmek için yüksek pageSize veriyoruz
    params.append('page', 1);
    params.append('pageSize', 1000);

    const projectInput = document.getElementById('filterProjectInput');
    const userInput = document.getElementById('filterUserInput');
    const statusSelect = document.getElementById('filterStatus');
    const prioritySelect = document.getElementById('filterPriority');

    // İSTEĞE BAĞLI FİLTRELER (Sadece doldurulmuşsa isteğe eklenir)
    if (projectInput?.value.trim()) {
        const matchedProject = allProjects.find(p => p.name?.trim().toLowerCase() === projectInput.value.trim().toLowerCase());
        if (matchedProject) params.append('projectId', matchedProject.id);
    }
    
    if (userInput?.value.trim()) {
        const matchedUser = allUsers.find(u => `${u.firstName} ${u.lastName}`?.trim().toLowerCase() === userInput.value.trim().toLowerCase());
        if (matchedUser) params.append('assignedToUserId', matchedUser.id);
    }

    if (statusSelect?.value !== "" && statusSelect?.value !== null && statusSelect?.value !== undefined) {
        params.append('status', statusSelect.value);
    }

    if (prioritySelect?.value !== "" && prioritySelect?.value !== null && prioritySelect?.value !== undefined) {
        params.append('priority', prioritySelect.value);
    }

    try {
        const url = `${baseUrl}/Tasks?${params.toString()}`;
        console.log("İstek atılan URL:", url);

        const response = await fetch(url, {
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            console.error("API Hatası! Durum Kodu:", response.status);
            return;
        }

        const result = await response.json();
        
        let tasksData = [];
        if (Array.isArray(result)) {
            tasksData = result;
        } else if (result.data && Array.isArray(result.data)) {
            tasksData = result.data;
        } else if (result.items && Array.isArray(result.items)) {
            tasksData = result.items;
        }

        // Hem Kanban Panosunu hem de Tablo varsa Tabloyu güncelle
        renderKanbanBoard(tasksData);
        renderTable(tasksData);

    } catch (error) {
        console.error("Görevler listelenirken JS bağlantı hatası:", error);
    }
}

async function loadProjectMembers() {

    const projectName = document.getElementById("addProjectId").value.trim();

    const project = allProjects.find(p =>
        p.name.toLowerCase() === projectName.toLowerCase());

    if (!project) return;

    try {

        const response = await fetch(
            `${baseUrl}/Projects/${project.id}/members`,
            {
                headers: {
                    "Authorization": "Bearer " + localStorage.getItem("authToken")
                }
            });

        if (!response.ok) return;

        projectMembers = await response.json();

        const list = document.getElementById("addUserOptions");
        list.innerHTML = "";

        projectMembers.forEach(member => {

            list.innerHTML += `
                <option value="${member.firstName} ${member.lastName}">
            `;

        });
    }
    catch (err) {

        console.log(err);
    }
}

// KANBAN PANOSUNA VERİLERİ BASTIRAN FONKSİYON
function renderKanbanBoard(tasks) {
    // Önce tüm Kanban kolonlarını ve sayaçlarını sıfırla
    [0, 1, 2, 3].forEach(status => {
        const container = document.getElementById(`kanbanStatus${status}`);
        const badge = document.getElementById(`countStatus${status}`);
        if (container) container.innerHTML = '';
        if (badge) badge.innerText = '0';
    });

    if (!tasks || tasks.length === 0) return;

    const counts = { 0: 0, 1: 0, 2: 0, 3: 0 };

    tasks.forEach(task => {
        const status = task.status !== undefined ? task.status : 0;
        const container = document.getElementById(`kanbanStatus${status}`);
        
        if (container) {
            counts[status] = (counts[status] || 0) + 1;

            // Proje Adı Bulma
            const targetProjId = task.projectId || task.ProjeId || task.projectID;
            let projectName = 'Projesiz';
            if (targetProjId && allProjects.length > 0) {
                const foundProj = allProjects.find(p => p.id === targetProjId);
                if (foundProj) projectName = foundProj.name;
            }

            // Atanan Kişi Bulma
            const targetUserId = task.assignedToUserId || task.UserId || task.userId;
            let assignedUserText = 'Atanmadı';
            if (targetUserId && allUsers.length > 0) {
                const foundUser = allUsers.find(u => u.id === targetUserId);
                if (foundUser) assignedUserText = `${foundUser.firstName} ${foundUser.lastName}`;
            }

            const priorityClasses = ['priority-low', 'priority-medium', 'priority-high', 'priority-critical'];
            const priorityTexts = ['Düşük', 'Orta', 'Yüksek', 'Kritik'];
            const priorityClass = priorityClasses[task.priority] || 'priority-low';
            const priorityText = priorityTexts[task.priority] || 'Düşük';

            const cardHTML = `
                <div class="task-card ${task.status == 3 ? 'completed-card' : ''}">
    
    <div class="card-header">
        <span class="project-tag">${projectName}</span>
        <span class="priority-badge ${priorityClass}">${priorityText}</span>
    </div>

    <h4 class="card-title">${task.title || 'Başlıksız Görev'}</h4>

    <p class="card-desc">${task.description || ''}</p>

    <div class="card-meta-row">
        <span class="meta-item">👤 ${assignedUserText}</span>
        <span class="meta-item">📅 ${
            task.dueDate
            ? new Date(task.dueDate).toLocaleDateString("tr-TR")
            : "-"
        }</span>
    </div>

    <div class="card-actions">

        <button class="action-icon-btn btn-view"
onclick="event.stopPropagation(); openTaskModal(
'${task.id}',
'${escapeQuotes(task.title)}',
'${escapeQuotes(task.description||'')}',
${task.status},
${task.priority},
'${task.assignedToUserId||''}',
'${task.projectId}')">
            🖋️
        </button>

            <button class="action-icon-btn btn-time"
onclick="event.stopPropagation(); openTimeLogModal('${task.id}','${escapeQuotes(task.title)}')">
    ⏱️
</button>

<button class="action-icon-btn btn-history"
onclick="event.stopPropagation(); openHistoryModal('${task.id}','${escapeQuotes(task.title)}')">
    📜
</button>

      <button class="action-icon-btn btn-delete"
    onclick="event.stopPropagation(); openDeleteModal('${task.id}')">
    🗑
</button>

    </div>

</div>
            `;
            container.innerHTML += cardHTML;
        }
    });

    // Kolon başlıklarındaki sayıları güncelle
    Object.keys(counts).forEach(status => {
        const badge = document.getElementById(`countStatus${status}`);
        if (badge) badge.innerText = counts[status];
    });
}

let selectedTaskIdToDelete = null;

function openDeleteModal(taskId) {
    selectedTaskIdToDelete = taskId;
    const deleteModal = document.getElementById('deleteModal');
    if (deleteModal) {
        deleteModal.style.display = 'flex';
    }
}

async function deleteTask(taskId) {

    try {
        const response = await fetch(`https://localhost:7075/api/Tasks/${taskId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            // Tabloyu veya kanban kartlarını yenile
            if (typeof fetchTasks === 'function') fetchTasks();
        } else {
            alert("Silme işlemi başarısız oldu! Hata kodu: " + response.status);
        }
    } catch (error) {
        console.error("Silme hatası:", error);
    }
}

// TABLO YAPISI VARSA BASTIRAN FONKSİYON (Yedek/Opsiyonel)
function renderTable(tasks) {
    const tableBody = document.getElementById('taskListTableBody');
    if (!tableBody) return;

    tableBody.innerHTML = '';

    if (!tasks || tasks.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="7" style="text-align:center;">Görev bulunamadı.</td></tr>';
        return;
    }

    tasks.forEach(task => {
        const dueDate = task.dueDate ? new Date(task.dueDate).toLocaleDateString('tr-TR') : '-';
        let statusText = ["Planlandı", "Devam Ediyor", "İncelemede", "Tamamlandı"][task.status] || "Bilinmiyor";
        let priorityText = ["Düşük", "Orta", "Yüksek", "Kritik"][task.priority] || "Bilinmiyor";

        const targetProjId = task.projectId || task.ProjeId || task.projectID;
        let projectName = '-';
        if (targetProjId && allProjects.length > 0) {
            const foundProj = allProjects.find(p => p.id === targetProjId);
            projectName = foundProj ? foundProj.name : targetProjId;
        }

        const targetUserId = task.assignedToUserId || task.UserId || task.userId;
        let assignedUserText = 'Atanmadı';
        if (targetUserId && allUsers.length > 0) {
            const foundUser = allUsers.find(u => u.id === targetUserId);
            if (foundUser) assignedUserText = `${foundUser.firstName} ${foundUser.lastName}`;
        }

        const row = `<tr>
            <td>${task.title || '-'}</td>
            <td>${projectName}</td>
            <td>${assignedUserText}</td>
            <td>${statusText}</td>
            <td>${priorityText}</td>
            <td>${dueDate}</td>
            <td>
                <button class="edit-btn" 
                style="background: none; border: none; cursor: pointer; font-size: 18px; color: #007bff;" 
                onclick="openTaskModal(
'${task.id}',
'${escapeQuotes(task.title)}',
'${escapeQuotes(task.description || '')}',
${task.status},
${task.priority},
'${task.assignedToUserId || ''}',
'${task.projectId}')"
                >Düzenle</button>
            </td>
        </tr>`;
        tableBody.innerHTML += row;
    });
}

function applyTaskFilter() {
    fetchTasks();
}

async function filterUsersByProject() {
    const projectInput = document.getElementById('filterProjectInput');
    const userDatalist = document.getElementById('userOptions');
    
    if (!projectInput || !userDatalist) return;

    if (projectInput.value.trim() === "") {
        userDatalist.innerHTML = '';
        allUsers.forEach(u => {
            userDatalist.innerHTML += `<option value="${u.firstName} ${u.lastName}">`;
        });
        return;
    }

    const matchedProject = allProjects.find(p => p.name.trim().toLowerCase() === projectInput.value.trim().toLowerCase());
    
    if (matchedProject) {
        try {
            const response = await fetch(`${baseUrl}/Tasks?projectId=${matchedProject.id}&page=1&pageSize=1000`, {
                headers: { 
                    'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const result = await response.json();
                let projectTasks = [];
                
                if (Array.isArray(result)) projectTasks = result;
                else if (result.data && Array.isArray(result.data)) projectTasks = result.data;
                else if (result.items && Array.isArray(result.items)) projectTasks = result.items;

                const assignedUserIds = [...new Set(projectTasks.map(t => t.assignedToUserId || t.UserId || t.userId).filter(id => id != null))];

                userDatalist.innerHTML = '';
                allUsers.forEach(u => {
                    if (assignedUserIds.includes(u.id)) {
                        userDatalist.innerHTML += `<option value="${u.firstName} ${u.lastName}">`;
                    }
                });
            }
        } catch (error) {
            console.error("Proje kullanıcıları filtrelenirken hata oluştu:", error);
        }
    }
}

async function openTaskModal(taskId, title, description, currentStatus, currentPriority, assignedUserId, projectId) {

    const modal = document.getElementById('editTaskModal');
    if (modal) modal.style.display = 'flex';

    document.getElementById('editTaskId').value = taskId;
    document.getElementById('editTaskTitle').value = title;
    document.getElementById('editTaskDescription').value = description;
    document.getElementById('editTaskStatus').value = currentStatus;
    document.getElementById('editTaskPriority').value = currentPriority;

    const datalist = document.getElementById('editUserOptions');
    const userInputField = document.getElementById('editTaskUser');

    datalist.innerHTML = "";
    userInputField.value = "";

    try {

        const response = await fetch(
            `${baseUrl}/Projects/${projectId}/members`,
            {
                headers: {
                    "Authorization": "Bearer " + localStorage.getItem("authToken")
                }
            });

        if (!response.ok) return;

        projectMembers = await response.json();

        projectMembers.forEach(member => {

            datalist.innerHTML += `<option value="${member.firstName} ${member.lastName}">`;

            if (member.userId === assignedUserId || member.id === assignedUserId) {
                userInputField.value = `${member.firstName} ${member.lastName}`;
            }

        });

    }
    catch (err) {
        console.log(err);
    }

    document.getElementById('editTaskMessage').innerText = "";
}

function closeTaskModal() {
    const modal = document.getElementById('editTaskModal');
    if(modal) modal.style.display = 'none';
}
// Butona basıldığında modalı açan fonksiyon
function openAddTaskModal() {
    const modal = document.getElementById('addTaskModal');
    if (modal) {
        modal.style.display = 'flex'; // Ekranın ortasında gösterir
    } else {
        console.error("HATA: 'addTaskModal' ID'li element HTML'de bulunamadı!");
    }
}

// Modal kapama fonksiyonu
function closeAddTaskModal() {
    const modal = document.getElementById('addTaskModal');
    if (modal) {
        modal.style.display = 'none';
    }
    const form = document.getElementById('createTaskForm');
    if (form) form.reset();
}

function closeDeleteModal() {
    const deleteModal = document.getElementById('deleteModal');
    if (deleteModal) deleteModal.style.display = 'none';
    selectedTaskIdToDelete = null;
}

async function confirmDeleteTask() {
    if (!selectedTaskIdToDelete) return;
    
    // Doğrudan API'ye silme isteği atan fonksiyonunu çağırır
    await deleteTask(selectedTaskIdToDelete);
    
    closeDeleteModal();
}

async function saveTaskChanges() {
    const taskId = document.getElementById('editTaskId').value;
    const title = document.getElementById('editTaskTitle').value.trim();
    const description = document.getElementById('editTaskDescription').value.trim();
    const status = parseInt(document.getElementById('editTaskStatus').value);
    const priority = parseInt(document.getElementById('editTaskPriority').value);
    const typedUserName = document.getElementById('editTaskUser').value.trim();
    const messageLabel = document.getElementById('editTaskMessage');

    let assignedToUserId = null;
    if (typedUserName) {
        const foundUser = projectMembers.find(u =>
    `${u.firstName} ${u.lastName}`.toLowerCase() === typedUserName.toLowerCase());
        if (foundUser) {
            assignedToUserId = foundUser.userId;
        }
    }

    if(messageLabel) {
        messageLabel.innerText = "Kaydediliyor...";
        messageLabel.style.color = "gray";
    }

    try {
        const response = await fetch(`${baseUrl}/Tasks/${taskId}`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                title: title,
                description: description,
                status: status,
                priority: priority,
                assignedToUserId: assignedToUserId
            })
        });

        if (response.ok) {
            if(messageLabel) {
                messageLabel.innerText = "Güncellendi!";
                messageLabel.style.color = "#28a745";
            }
            setTimeout(() => {
                closeTaskModal();
                fetchTasks();
            }, 800);
        } else {
            const errorText = await response.text();
            if(messageLabel) {
                messageLabel.innerText = "Başarısız! Kod: " + response.status;
                messageLabel.style.color = "#dc3545";
            }
            console.error("Güncelleme hatası detay:", errorText);
        }
    } catch (error) {
        console.error("Güncelleme hatası:", error);
        if(messageLabel) {
            messageLabel.innerText = "Sunucu hatası!";
            messageLabel.style.color = "#dc3545";
        }
    }
}

function toggleSidebar() {
    const sidebar = document.getElementById("mySidebar");
    const content = document.querySelector(".content-wrapper");
    if (!sidebar || !content) return;

    if (sidebar.style.width === "250px") {
        sidebar.style.width = "0";
        content.style.marginLeft = "80px";
        content.style.width = "calc(100% - 80px)";
    } else {
        sidebar.style.width = "250px";
        content.style.marginLeft = "250px";
        content.style.width = "calc(100% - 250px)";
    }
}


function escapeQuotes(str) {
    if (!str) return '';
    return str.replace(/'/g, "\\'").replace(/"/g, '&quot;');
}

async function createTask(event) {
    event.preventDefault();

    const title = document.getElementById("addTitle").value.trim();
    const description = document.getElementById("addDescription").value.trim();
    const priority = parseInt(document.getElementById("addPriority").value);
    const dueDate = document.getElementById("addDueDate").value;

    const projectName = document.getElementById("addProjectId").value.trim();
    const userName = document.getElementById("addAssignedToUserId").value.trim();

    const message = document.getElementById("addResultMessage");

    if (!title) {
        message.innerText = "Görev adı zorunludur.";
        message.style.color = "red";
        return;
    }

    const project = allProjects.find(p =>
        p.name.toLowerCase() === projectName.toLowerCase());

    if (!project) {
        message.innerText = "Geçerli bir proje seçiniz.";
        message.style.color = "red";
        return;
    }

    let assignedUserId = null;

    if (userName !== "") {

        const user = projectMembers.find(u =>
            `${u.firstName} ${u.lastName}`.toLowerCase() === userName.toLowerCase());

        if (user)
            assignedUserId = user.userId;
    }

    const request = {
        title: title,
        description: description,
        projectId: project.id,
        assignedToUserId: assignedUserId,
        priority: priority,
        dueDate: dueDate === "" ? null : dueDate,
        estimatedHours: null
    };

    try {

        const response = await fetch(`${baseUrl}/Tasks`, {

            method: "POST",

            headers: {
                "Authorization": "Bearer " + localStorage.getItem("authToken"),
                "Content-Type": "application/json"
            },

            body: JSON.stringify(request)

        });

        if (response.ok) {

            message.innerText = "Görev başarıyla oluşturuldu.";
            message.style.color = "green";

            closeAddTaskModal();

            fetchTasks();

        }
        else {

            const error = await response.text();

            console.log(error);

            message.innerText = "Görev oluşturulamadı.";
            message.style.color = "red";
        }
    }
    catch (err) {

        console.log(err);
        message.innerText = "Sunucu hatası.";
        message.style.color = "red";
    }
}

function openTimeLogModal(taskId, title) {

    document.getElementById("timeLogTaskId").value = taskId;
    document.getElementById("timeLogTaskTitle").innerText = title;

    document.getElementById("timeLogModal").style.display = "flex";

    loadTaskHistory(taskId);

}

function closeTimeLogModal(){
    document.getElementById("timeLogModal").style.display="none";
}

function openTimeLogModal(taskId, title) {

    document.getElementById("timeLogTaskId").value = taskId;
    document.getElementById("timeLogTaskTitle").innerText = title;

    document.getElementById("timeLogModal").style.display = "flex";
}

function closeTimeLogModal() {

    document.getElementById("timeLogModal").style.display = "none";

}

async function saveTimeLog() {

    const taskId = document.getElementById("timeLogTaskId").value;
    const workDate = document.getElementById("logDate").value;
    const workedHours = parseFloat(document.getElementById("workedHours").value);
    const description = document.getElementById("timeDescription").value;

    const message = document.getElementById("timeLogMessage");

    message.innerText = "";
    message.style.color = "";

    try {

        const response = await fetch(`${baseUrl}/tasks/${taskId}/time-logs`, {
            method: "POST",
            headers: {
                "Authorization": "Bearer " + localStorage.getItem("authToken"),
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                hours: workedHours,
                description: description,
                workDate: workDate
            })
        });

        const result = await response.json();

        if (response.ok) {
            loadTaskHistory(taskId);
            message.style.color = "green";
            message.innerText = result.message;

            setTimeout(() => {

                closeTimeLogModal();

                message.innerText = "";

            },1000);

        }
        else {

            message.style.color = "red";
            message.innerText = result.message;

        }

    }
    catch(err){

        console.log(err);

        message.style.color = "red";
        message.innerText = "Sunucuya bağlanılamadı.";

    }

}

async function loadTaskHistory(taskId) {

    const container = document.getElementById("taskHistoryList");

    container.innerHTML = "Yükleniyor...";

    try {

        const response = await fetch(
            `${baseUrl}/tasks/${taskId}/histories?page=1&pageSize=20`,
            {
                headers: {
                    "Authorization": "Bearer " + localStorage.getItem("authToken")
                }
            });

        if (!response.ok) {

            container.innerHTML = "Geçmiş bulunamadı.";
            return;
        }

        const histories = await response.json();

        container.innerHTML = "";

        histories.forEach(h => {

            container.innerHTML += `
                <div style="padding:8px;border-bottom:1px solid #ddd;">
                    <b>${
    h.changeType === "AssignedUserChanged" ? "👤 Atanan Kullanıcı Değiştirildi" :
    h.changeType === "StatusChanged" ? "📌 Durum Değiştirildi" :
    h.changeType === "PriorityChanged" ? "⚡ Öncelik Değiştirildi" :
    h.changeType === "TaskCreated" ? "🆕 Görev Oluşturuldu" :
    h.changeType
}</b><br>
                    ${h.description ?? ""}<br>
                    <small>${new Date(h.createdAt).toLocaleString("tr-TR")}</small>
                </div>
            `;

        });

    }
    catch {

        container.innerHTML = "Sunucu hatası.";

    }

}

function openHistoryModal(taskId, title){

    document.getElementById("historyTaskTitle").innerText = title;

    document.getElementById("historyModal").style.display = "flex";

    loadTaskHistory(taskId);

}

function closeHistoryModal(){

    document.getElementById("historyModal").style.display = "none";

}

async function loadTaskHistory(taskId){

    const container = document.getElementById("historyList");

    container.innerHTML = "Yükleniyor...";

    try{

        const response = await fetch(
            `${baseUrl}/tasks/${taskId}/histories?page=1&pageSize=20`,
            {
                headers:{
                    "Authorization":"Bearer "+localStorage.getItem("authToken")
                }
            });

        if(!response.ok){

            container.innerHTML="Geçmiş bulunamadı.";

            return;
        }

        const histories = await response.json();

        container.innerHTML="";

        histories.forEach(h=>{

            container.innerHTML += `
                <div style="padding:10px;border-bottom:1px solid #ddd;">
                    <b>${h.changeType}</b><br>

                    ${h.description ?? ""}<br>

                    <small>${new Date(h.createdAt).toLocaleString("tr-TR")}</small>
                </div>
            `;

        });

    }
    catch{

        container.innerHTML="Sunucu hatası.";

    }

}


function logout() {
    localStorage.removeItem("token");
    window.location.href = "../../Login/index.html";
}
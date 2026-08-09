let currentPage = 1;
const pageSize = 10;
let currentProjectId = null;
let allUsers = [];
const BASE_URL = "https://localhost:7075";
async function fetchProjects() {
    const statusValue = document.getElementById('statusFilter').value;
    const ownerIdValue = document.getElementById('ownerIdFilter').value;

    const baseUrl = 'https://localhost:7075/api/Projects';
    const params = new URLSearchParams({
        page: currentPage,
        pageSize: pageSize
    });

    if (statusValue !== "") params.append('status', statusValue);
    if (ownerIdValue !== "") params.append('ownerId', ownerIdValue);

    try {
        const url = `${baseUrl}?${params.toString()}`;
        
        const response = await fetch(url, {
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            console.error("Backend'den hata döndü! Hata Kodu:", response.status);
            document.getElementById('projectListTableBody').innerHTML = `<tr><td colspan="6">Veri çekilemedi. Hata kodu: ${response.status}</td></tr>`;
            return; 
        }
        
        const result = await response.json();
        const projectsData = result.data ? result.data : result; 
        
        renderTable(projectsData); 
        updatePaginationControls(projectsData.length);

    } catch (error) {
        console.error("Hata oluştu:", error);
    }
}

function renderTable(projects) {
    const tableBody = document.getElementById('projectListTableBody');
    tableBody.innerHTML = '';

    if (!projects || projects.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6">Proje bulunamadı.</td></tr>';
        return;
    }

    projects.forEach(project => {
        const start = project.startDate ? new Date(project.startDate).toLocaleDateString('tr-TR') : '-';
        const end = project.endDate ? new Date(project.endDate).toLocaleDateString('tr-TR') : '-';

        let statusText = "Bilinmiyor";
        if (project.status === 0) statusText = "Planlandı";
        else if (project.status === 1) statusText = "Devam Ediyor";
        else if (project.status === 2) statusText = "Tamamlandı";

        const row = `<tr>
            <td>${project.name}</td>
            <td>${statusText}</td>
            <td>${start}</td>
            <td>${end}</td>
            <td>${project.ownerId}</td>
            <td>
                <button class="edit-btn" onclick="openModal('${project.id}')">Düzenle</button>
                <button class="delete-btn" onclick="openDeleteModal('${project.id}')" style="margin-left: 5px;">Sil</button>
                <button class="detail-btn" onclick="openDetailModal('${project.id}')" style="margin-left: 5px;">Detay Görüntüle</button>
                <button class="member-btn" onclick="openMembersModal('${project.id}','${project.name}')">Üyeler</button>
            </td>
            </tr>`;
         
        tableBody.innerHTML += row;
    });
}

async function openMembersModal(projectId, projectName) {
    currentProjectId = projectId;
    document.getElementById("membersProjectTitle").innerText =
        projectName + " Üyeleri";
    document.getElementById("membersModal").style.display = "flex";
    loadProjectMembers();
}

function closeMembersModal() {
    document.getElementById("membersModal").style.display = "none";
}

async function loadProjectMembers() {
    const response = await fetch(
        `${BASE_URL}/api/projects/${currentProjectId}/members`,
        {
            headers:{
                Authorization:"Bearer "+localStorage.getItem("authToken")
            }
        });

    const members = await response.json();
    console.log(members);
    const container =
        document.getElementById("membersContainer");

    container.innerHTML="";
    members.forEach(member=>{
        let roleText = "";

if(member.role == 0)
    roleText = "Member";

if(member.role == 1)
    roleText = "Contributor";

if(member.role == 2)
    roleText = "Viewer";

container.innerHTML += `
<div class="member-card">

    <div class="member-left">
        <div class="avatar">
            ${member.firstName.charAt(0)}${member.lastName.charAt(0)}
        </div>

        <div class="member-info">
            <div class="member-name">
                ${member.firstName} ${member.lastName}
            </div>

            <div class="member-role">
                ${getRoleName(member.role)}
            </div>
        </div>
    </div>

    <button
        class="remove-member-btn"
        onclick="removeMember('${member.userId}')">
        Çıkar
    </button>

</div>
`;
    });
}

function getRoleName(role){
    switch(role){
        case 0:
            return "Member";
        case 1:
            return "Contributor";
        case 2:
            return "Viewer";
        default:
            return "Bilinmiyor";
    }
}

async function openAddMemberModal(){
   document.getElementById("addMemberModal").style.display="flex";
    await loadUsers();
}

async function loadUsers(){

    const response = await fetch(
        `${BASE_URL}/api/users?page=1&pageSize=200`,
        {
            headers:{
                Authorization:"Bearer "+localStorage.getItem("authToken")
            }
        });

    console.log("Status:", response.status);

    const result = await response.json();
    console.log("Result:", result);

    allUsers = result.data || result;

    console.log("Users:", allUsers);

    renderUsers(allUsers);
}

function renderUsers(users){

    const select = document.getElementById("userSelect");

    select.innerHTML = "<option value=''>Kullanıcı Seçiniz</option>";

    users.forEach(user => {

        select.innerHTML += `
            <option value="${user.id}">
                ${user.firstName} ${user.lastName}
            </option>
        `;

    });

}


function applyFilter() {
    currentPage = 1; 
    fetchProjects();
}

function nextPage() {
    currentPage++;
    fetchProjects(); 
}

function prevPage() {
    if (currentPage > 1) {
        currentPage--;
        fetchProjects(); 
    }
}

function updatePaginationControls(currentDataLength) {
    document.getElementById('pageInfo').innerText = `Sayfa ${currentPage}`;
    document.getElementById('prevBtn').disabled = (currentPage === 1);
    document.getElementById('nextBtn').disabled = (currentDataLength < pageSize);
}

function openAddModal() {
    document.getElementById('addProjectModal').style.display = 'flex';
}

function closeAddModal() {
    document.getElementById('addProjectModal').style.display = 'none';
    document.getElementById('createProjectForm').reset();
    const resultMessage = document.getElementById('resultMessage');
    if (resultMessage) resultMessage.innerText = '';
}

async function createProject(event) {
    event.preventDefault();

    const resultMessage = document.getElementById('resultMessage');
    const startVal = document.getElementById('addStartDate').value;
    const endVal = document.getElementById('addEndDate').value;

    const projectData = {
        name: document.getElementById('addName').value,
        description: document.getElementById('addDescription').value,
        startDate: startVal ? new Date(startVal).toISOString() : null,
        endDate: endVal ? new Date(endVal).toISOString() : null,
        status: parseInt(document.getElementById('addStatus').value)
    };

    const url = 'https://localhost:7075/api/Projects'; 

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(projectData)
        });

        if (response.ok) {
            resultMessage.style.color = "#28a745";
            resultMessage.innerText = "Proje başarıyla oluşturuldu.";
            document.getElementById('createProjectForm').reset();
            applyFilter();

            setTimeout(() => {
                resultMessage.innerText = ""; 
                closeAddModal();
            }, 1500);
        } else {
            const errorText = await response.text();
            resultMessage.style.color = "#dc3545";
            resultMessage.innerText = "Proje oluşturulamadı. Hata kodu: " + response.status;
            console.error("Backend hata detayı:", errorText);
        }
    } catch (error) {
        console.error("İstek hatası:", error);
        resultMessage.style.color = "#dc3545";
        resultMessage.innerText = "Sunucuya ulaşılamadı!";
    }
}

async function openModal(projectId) {
    const modal = document.getElementById('editModal');
    const messageLabel = document.getElementById('editMessage');
    messageLabel.innerText = "Bilgiler yükleniyor...";
    messageLabel.style.color = "white";
    
    modal.style.display = 'flex';
    document.getElementById('editProjectId').value = projectId;

    try {
        const response = await fetch(`https://localhost:7075/api/Projects/${projectId}`, {
            headers: { 'Authorization': 'Bearer ' + localStorage.getItem('authToken') }
        });

        if (!response.ok) throw new Error("Proje bilgileri alınamadı.");
        
        const project = await response.json();

        document.getElementById('editName').value = project.name;
        document.getElementById('editDescription').value = project.description || "";
        document.getElementById('editStatus').value = project.status;

        if (project.startDate) {
            document.getElementById('editStartDate').value = project.startDate.split('T')[0];
        }
        if (project.endDate) {
            document.getElementById('editEndDate').value = project.endDate.split('T')[0];
        }

        messageLabel.innerText = ""; 

    } catch (error) {
        console.error("Hata:", error);
        messageLabel.innerText = "Bilgiler çekilirken hata oluştu.";
        messageLabel.style.color = "red";
    }
}

async function saveChanges() {
    const projectId = document.getElementById('editProjectId').value;
    const messageLabel = document.getElementById('editMessage');

    const updatedData = {
        name: document.getElementById('editName').value,
        description: document.getElementById('editDescription').value,
        startDate: document.getElementById('editStartDate').value,
        endDate: document.getElementById('editEndDate').value,
        status: parseInt(document.getElementById('editStatus').value) 
    };

    try {
        const response = await fetch(`https://localhost:7075/api/Projects/${projectId}`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedData)
        });

        if (response.ok) {
            messageLabel.innerText = "Proje başarıyla güncellendi!";
            messageLabel.style.color = "#28a745";

            setTimeout(() => {
                closeModal();
                fetchProjects(); 
            }, 1500);
        } else {
            messageLabel.innerText = "Güncelleme başarısız! Hata kodu: " + response.status;
            messageLabel.style.color = "#dc3545"; 
        }
    } catch (error) {
        console.error("Hata:", error);
        messageLabel.innerText = "Sunucuya bağlanılamadı!";
    }
}

function closeModal() {
    document.getElementById('editModal').style.display = 'none';
    document.getElementById('editForm').reset(); 
    document.getElementById('editMessage').innerText = ""; 
}

function openDeleteModal(projectId) {
    document.getElementById('deleteModal').style.display = 'flex';
    document.getElementById('deleteProjectId').value=projectId;
    document.getElementById('deleteMessage').innerText="";
}

function closeDeleteModal(){
    document.getElementById('deleteModal').style.display='none';
}

async function confirmDelete(){
    const projectId= document.getElementById('deleteProjectId').value;
    console.log("API'ye gönderilen ID: ", projectId);
    const messageLabel = document.getElementById('deleteMessage');

    messageLabel.innerText="Siliniyor..."
    messageLabel.style.color="white";

try {
    const response = await fetch(`https://localhost:7075/api/Projects/${projectId}`, 
        {
        method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            }
        });

        if (response.ok){
            messageLabel.innerText="Proje başarıyla silindi.";
            messageLabel.style.color="#28a745";

            setTimeout(() =>
            {
                closeDeleteModal();
                fetchProjects();
            }, 1500);
        } 
        else
            {
            messageLabel.innerText="Silme başarısız.Hata:"+response.status;
            messageLabel.style.color = "#dc3545";
            }
    } catch (error)
        {
            console.error("Hata:", error);
            messageLabel.innerText ="Sunucuya bağlanılamadı.";
            messageLabel.style.color="#dc3545";
        }   
    }

    async function openDetailModal(projectId) {
        const modal = document.getElementById('projectDetailModal');
    
        document.getElementById('detailName').innerText = "Yükleniyor...";
        document.getElementById('detailDescription').innerText = "...";
        document.getElementById('detailStartDate').innerText = "...";
        document.getElementById('detailEndDate').innerText = "...";
        document.getElementById('detailStatus').innerText = "...";
        document.getElementById('detailOwnerId').innerText = "...";

    modal.style.display = 'flex';

    try {
        const response = await fetch(`https://localhost:7075/api/Projects/${projectId}`, {
            headers: { 'Authorization': 'Bearer ' + localStorage.getItem('authToken') }
        });
        if (!response.ok) throw new Error("Proje detayları alınamadı.");
        
        const project = await response.json();
        const start = project.startDate ? new Date(project.startDate).toLocaleDateString('tr-TR') : '-';
        const end = project.endDate ? new Date(project.endDate).toLocaleDateString('tr-TR') : '-';
        
        let statusText = "Bilinmiyor";
        if (project.status === 0) statusText = "Planlandı";
        else if (project.status === 1) statusText = "Devam Ediyor";
        else if (project.status === 2) statusText = "Tamamlandı";

        document.getElementById('detailName').innerText = project.name;
        document.getElementById('detailDescription').innerText = project.description || "Açıklama bulunmuyor.";
        document.getElementById('detailStartDate').innerText = start;
        document.getElementById('detailEndDate').innerText = end;
        document.getElementById('detailStatus').innerText = statusText;
        document.getElementById('detailOwnerId').innerText = project.ownerId;

    } catch (error) {
        console.error("Hata:", error);
        document.getElementById('detailName').innerText = "Veri çekilemedi!";
        document.getElementById('detailName').style.color = "red";
    }
}

function closeDetailModal() {
    document.getElementById('projectDetailModal').style.display = 'none';
}

function closeAddMemberModal() {
    document.getElementById("addMemberModal").style.display = "none";
}

async function removeMember(memberId) 
{
    document.getElementById("removeMemberId").value = memberId;
    document.getElementById("removeMemberMessage").innerText = "";
    document.getElementById("removeMemberModal").style.display = "flex";
}

function closeRemoveMemberModal()
{
    document.getElementById("removeMemberModal").style.display = "none";
}

async function confirmRemoveMember()
{
    const memberId =
        document.getElementById("removeMemberId").value;

    const message =
        document.getElementById("removeMemberMessage");

    try{

        const response = await fetch(
            `${BASE_URL}/api/projects/${currentProjectId}/members/${memberId}`,
            {
                method:"DELETE",
                headers:{
                    Authorization:"Bearer "+localStorage.getItem("authToken")
                }
            });

        if(response.ok){
            message.style.color="#28a745";
            message.innerText="Üye başarıyla çıkarıldı.";
            setTimeout(()=>{
                closeRemoveMemberModal();
                loadProjectMembers();
            },1000);

        }else{
            const error = await response.text();
            message.style.color="red";
            message.innerText=error;
        }
    }catch{

        message.style.color="red";
        message.innerText="Bir hata oluştu.";
    }
}
  

async function addMember() {

    const userId = document.getElementById("userSelect").value;
    const role = parseInt(document.getElementById("memberRole").value);

    if (userId === "") {
        alert("Lütfen kullanıcı seçiniz.");
        return;
    }

    const response = await fetch(
        `${BASE_URL}/api/projects/${currentProjectId}/members`,
        {
            method: "POST",
            headers: {
                "Authorization": "Bearer " + localStorage.getItem("authToken"),
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                userId: userId,
                role: role
            })
        });

    if (response.ok) {
        alert("Üye eklendi.");
        closeAddMemberModal();
        loadProjectMembers();

    } else {

        const error = await response.text();
        console.log(error);
        alert(error);

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


function logout() {
    localStorage.removeItem("token");
    window.location.href = "../../Login/index.html";
}

let currentPage = 1;
const pageSize = 10;

async function filterUsers() {

    const name=document.getElementById('searchNameEmail').value.trim();
    const role=document.getElementById('roleFilter').value;
    const status=document.getElementById('statusFilter').value;

    let backendStatus = status;
    if (status === "active") backendStatus = "true";
    if (status === "passive") backendStatus = "false";

    const sortBy='CreatedAt';
    const sortDirection='desc';

    const baseUrl='https://localhost:7075/api/users';
    const params= new URLSearchParams({
        FirstName: name,
        role: role,
        isActive: backendStatus,
        sortBy: 'CreatedAt',
        sortDirection: 'desc',
        page: currentPage,
        pageSize:pageSize
    });

    if (name === "") params.delete('FirstName');
    if (role === "") params.delete('role');
    if (backendStatus === "") params.delete('IsActive');

    try {
        const url=`${baseUrl}?${params.toString()}`;
        
        
        const response= await fetch(url, {
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
            'Content-Type': 'application/json'
        }
    });

    if (!response.ok) {
            console.error("Backend'den hata döndü! Hata Kodu:", response.status);
            document.getElementById('userListTableBody').innerHTML = `<tr><td colspan="5">Veri çekilemedi. Hata kodu: ${response.status}</td></tr>`;
            return;
        }
        
    const result = await response.json();
    const usersData = result.data ? result.data : result; 
    renderTable(usersData); 
    updatePaginationControls(usersData.length);

    } catch (error) {
        console.error("Hata oluştu:", error);
    }
}

    function renderTable(users) {
        const tableBody = document.getElementById('userListTableBody');
        tableBody.innerHTML='';

        if (!users || users.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5">Veri bulunamadı.</td></tr>';
        return;
    }

        users.forEach(user => {
            const row = `<tr>
            <td>${user.firstName}</td>
            <td>${user.lastName}</td>
            <td>${user.email}</td>
            <td>${user.role}</td>
            <td>
                <span id="statusText-${user.id}">${user.isActive ? 'Aktif' : 'Pasif'}</span>
                
                <button onclick="toggleUserStatus('${user.id}', ${user.isActive})" 
                        style="background: none; border: none; cursor: pointer; font-size: 18px; color: #007bff; margin-left: 10px;" 
                        title="Durumu Değiştir"> 🗘
                </button>
            </td>
        </tr>`;
        tableBody.innerHTML+=row;
        });
    }
function applyFilter() {
    currentPage = 1;
    filterUsers();
}

function nextPage() {
    currentPage++;
    filterUsers();
}

function prevPage() {
    if (currentPage > 1) {
        currentPage--;
        filterUsers();
    }
}

async function toggleUserStatus(id, currentIsActive) {

    const newStatus = !currentIsActive; 


    const url = `https://localhost:7075/api/Users/${id}/toggle-status`; 

    try {
        const response = await fetch(url, {
            method: 'PATCH',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ isActive: newStatus }) 
        });

        if (response.ok) {
            filterUsers(); 
        } else {
            alert("Durum güncellenemedi! Hata Kodu: " + response.status);
        }
    } catch (error) {
        console.error("Durum değiştirme hatası:", error);
        alert("Sunucuya bağlanılamadı!");
    }
}

function updatePaginationControls(currentDataLength) {
    document.getElementById('pageInfo').innerText = `Sayfa ${currentPage}`;
    document.getElementById('prevBtn').disabled = (currentPage === 1);
    document.getElementById('nextBtn').disabled = (currentDataLength < pageSize);
}

function openAddUserModal() {
    const modal = document.getElementById('addUserModal');
    if (modal) {
        modal.style.display = 'flex';
    }
}

function closeAddUserModal() {
    const modal = document.getElementById('addUserModal');
    if (modal) {
        modal.style.display = 'none';
    }
    const form = document.getElementById('createUserForm');
    if (form) {
        form.reset();
    }
    const messageLabel = document.getElementById('userResultMessage');
    if (messageLabel) {
        messageLabel.innerText = '';
    }
}


async function AddRegister(event) {
    event.preventDefault();

    const userData = {
        firstName: document.getElementById('newFirstName').value,
        lastName: document.getElementById('newLastName').value,
        email: document.getElementById('newEmail').value,
        password: document.getElementById('newPassword').value,
        role: parseInt(document.getElementById('newRole').value),
        isActive: true
    };

    try {
        const response = await fetch('https://localhost:7075/api/auth/register', {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('authToken'),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userData)
        });

        if (response.ok) {
            alert('Kullanıcı başarıyla eklendi!');
            closeAddUserModal();
            document.getElementById('addUserForm').reset();
            filterUsers();
        } else {
            alert('Kayıt başarısız! Hata kodu: ' + response.status);
        }
    } catch (error) {
        console.error('Bağlantı hatası:', error);
    }
}

    function toggleSidebarUserList() 
        {
            const sidebar = document.getElementById("mySidebar");
            const content = document.querySelector(".content-wrapper");
            const table = document.querySelector(".userList-table");

    if (!sidebar || !content) return;

    if (sidebar.style.width === "250px") {
        sidebar.style.width = "0";
        content.style.marginLeft = "80px";
        content.style.width = "calc(100% - 80px)";
    } 

    else {
        sidebar.style.width = "250px";
        content.style.marginLeft = "250px";
        content.style.width = "calc(100% - 250px)";
    }
}

function logout() {
    localStorage.removeItem("token");
    window.location.href = "../../Login/index.html";
}
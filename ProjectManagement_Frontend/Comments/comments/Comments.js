const BASE_URL = 'https://localhost:7075'; 

let currentPage = 1;
const pageSize = 10;
let allComments = [];
let selectedCommentId = null;

document.addEventListener('DOMContentLoaded', async () => {
    await loadAllDataFromDB();

    const filterSelect = document.getElementById('taskFilter');
    if (filterSelect) {
        filterSelect.addEventListener('change', handleFilterChange);
    }
});

async function loadAllDataFromDB() {

    renderMessage("Veriler yükleniyor...");

     const params = new URLSearchParams({
        page: currentPage,
        pageSize: pageSize
    });


    const headers = {
        "Authorization": "Bearer " + localStorage.getItem("authToken"),
        "Content-Type": "application/json"
    };

    try {
        const taskResponse = await fetch(
            `${BASE_URL}/api/tasks?page=1&pageSize=100`,
            { headers }
        );

        if (!taskResponse.ok)
            throw new Error("Görevler alınamadı");

        const taskResult = await taskResponse.json();
        const tasks = taskResult.data ? taskResult.data : taskResult;

        populateTaskDropdown(tasks);

        let comments = [];

        // Her görevin yorumlarını çek
        for (const task of tasks) {
            const response = await fetch(
                `${BASE_URL}/api/tasks/${task.id}/comments`,
                { headers }
            );
            
            if (!response.ok)
                continue;
            const result = await response.json();
            const taskComments = result.data ? result.data : result;
            console.log(taskComments[0]);
            taskComments.forEach(c => {
                comments.push({
                    ...c,
                    taskId: task.id,
                    taskTitle: task.title
                });
            });
        }
        allComments = comments;
        renderComments(allComments);

    }
    catch (err) {
        console.error(err);
        renderMessage("Veriler yüklenemedi.");
    }
}

// 2. AÇILIR MENÜYÜ (DROPDOWN) DİNAMİK DOLDURMA
function populateTaskDropdown(tasks) {
    const filterSelect = document.getElementById('taskFilter');
    if (!filterSelect) return;

    filterSelect.innerHTML = '<option value="">Tüm Görevler</option>';

    tasks.forEach(task => {
        const option = document.createElement('option');
        option.value = task.id;
        option.textContent = task.title || task.name || `Görev #${task.id}`;
        filterSelect.appendChild(option);
    });
}

// 3. SEÇİLEN GÖREVE GÖRE FİLTRELEME
function handleFilterChange(e) {
    const selectedTaskId = e.target.value;

    if (!selectedTaskId) {
        renderComments(allComments);
    } else {
        const filtered = allComments.filter(c => String(c.taskId) === String(selectedTaskId));
        renderComments(filtered);
    }
}

function renderComments(commentsList) {
   window.currentComments = commentsList;

    const container = document.getElementById('commentsContainer');
    if (!container) return;

    container.innerHTML = '';

    if (!commentsList || commentsList.length === 0) {
        renderMessage('Gösterilecek yorum bulunamadı.');
        return;
    }

    commentsList.forEach(comment => {
        // GUID veya ID alanını güvenli şekilde alıyoruz
        const commentId = comment.id || comment.Id;

        const card = document.createElement('div');
        card.className = 'comment-card';
        card.innerHTML = `
            <div class="comment-header">
                <span class="author-name">${comment.userName || comment.userFullName || 'Kullanıcı'}</span>
                <span class="task-link">
                    <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M10 13a5 5 0 0 0 7.54.54l3-3a5 5 0 0 0-7.07-7.07l-1.72 1.71"></path>
                        <path d="M14 11a5 5 0 0 0-7.54-.54l-3 3a5 5 0 0 0 7.07 7.07l1.71-1.71"></path>
                    </svg>
                    ${comment.taskTitle || 'Görev Bilgisi Yok'}
                </span>
                <span class="comment-time">
                    <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <circle cx="12" cy="12" r="10"></circle>
                        <polyline points="12 6 12 12 16 14"></polyline>
                    </svg>
                    ${formatDate(comment.createdAt || comment.createdDate)}
                </span>
            </div>
            <div class="comment-body">
                ${comment.content || comment.text || ''}
            </div>
            <div class="comment-actions">
                
                <button class="action-icon-btn btn-edit" title="Düzenle" onclick="openEditModal('${commentId}')">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
                        <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
                    </svg>
                </button>

                <button class="action-icon-btn btn-view" title="Görüntüle" onclick="openDetailModal('${commentId}')">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                        <circle cx="12" cy="12" r="3"></circle>
                    </svg>
                </button>

                <button class="action-icon-btn btn-delete" title="Sil" onclick="deleteComment('${commentId}')">
                    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <polyline points="3 6 5 6 21 6"></polyline>
                        <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                    </svg>
                </button>
            </div>
        `;
        container.appendChild(card);
    });
}

async function openEditModal(commentId) {
    const commentList = window.currentComments || (typeof allComments !== 'undefined' ? allComments : []);
    const comment = commentList.find(c => String(c.id || c.Id) === String(commentId));

    if (!comment) {
        alert("Düzenlenecek yorum bulunamadı.");
        return;
    }

    // Inputları doldur
    document.getElementById('editCommentId').value = comment.id || comment.Id;
    document.getElementById('editAuthorInput').value = comment.userName || comment.userFullName || '';
    document.getElementById('editContentTextarea').value = comment.content || comment.text || '';
    


    // Görevler listesini dropdown'a aktar
    const taskSelect = document.getElementById('editTaskSelect');
    if (taskSelect) {
        taskSelect.innerHTML = '';
        const mainSelect = document.getElementById('taskFilter');
        if (mainSelect) {
            Array.from(mainSelect.options).forEach(opt => {
                if (opt.value) { // Boş "Tüm Görevler" seçeneğini atla
                    const newOpt = document.createElement('option');
                    newOpt.value = opt.value;
                    newOpt.textContent = opt.textContent;
                    if (String(opt.value) === String(comment.taskId)) {
                        newOpt.selected = true;
                    }
                    taskSelect.appendChild(newOpt);
                }
            });
        }
    }

    // Modalı Aç
    const editModal = document.getElementById('editModal');
    if (editModal) editModal.classList.add('active');
}

function closeEditModal() {
    const editModal = document.getElementById('editModal');
    if (editModal) editModal.classList.remove('active');
}

function closeEditModalOnOutsideClick(event) {
    if (event.target.classList.contains('modal-overlay')) {
        closeEditModal();
    }
}

async function saveNewComment() {

    const taskId = document.getElementById('addTaskSelect').value;
    const content = document.getElementById('addContentTextarea').value.trim();

    if (!taskId || !content) {
        alert("Görev ve yorum alanı zorunludur.");
        return;
    }

    const token = localStorage.getItem("authToken");

    try {

        const response = await fetch(`${BASE_URL}/api/tasks/${taskId}/comments`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                content: content
            })
        });


        if (response.ok) {

            alert("Yorum başarıyla eklendi.");

            document.getElementById('addContentTextarea').value = "";

            closeAddModal();

            await loadAllDataFromDB();

        } 
        else {

            const errorText = await response.text();
            console.log("Backend hata:", errorText);

            alert("Yorum eklenemedi: " + errorText);

        }

    } 
    catch(error) {

        console.error("Fetch hatası:", error);
        alert("Sunucuya bağlanılamadı.");

    }
}

// BACKEND'E DÜZENLEME İSTEĞİ (PUT /api/comments/{id})
async function saveEditedComment() {
    const commentId = document.getElementById('editCommentId').value;
    const content = document.getElementById('editContentTextarea').value;

    const payload = {
        content: content,
    };

    try {
       const response = await fetch(`${BASE_URL}/api/comments/${commentId}`, {
    method: "PUT",
    headers: {
        "Authorization": "Bearer " + localStorage.getItem("authToken"),
        "Content-Type": "application/json"
    },
    body: JSON.stringify(payload)
});

        if (response.ok) {
    closeEditModal();
    await loadAllDataFromDB();
} else {
    const error = await response.text();
    console.log(error);
}
    } catch (error) {
        console.error('Güncelleme hatası:', error);
        alert('Sunucuyla bağlantı kurulamadı.');
    }
}

// ==========================================
// YORUM DETAY MODAL FONKSİYONLARI
// ==========================================

// DETAY PENCERESİNİ AÇMA
function openDetailModal(commentId) {
    // Sayfadaki yorum verisini bul
    const commentList = window.currentComments || (typeof allComments !== 'undefined' ? allComments : []);
    const comment = commentList.find(c => String(c.id || c.Id) === String(commentId));

    if (!comment) {
        console.error("Yorum bulunamadı:", commentId);
        return;
    }

    // Modal içindeki alanları doldur
    const authorEl = document.getElementById('modalAuthor');
    const taskEl = document.getElementById('modalTask');
    const contentEl = document.getElementById('modalContent');
    const dateEl = document.getElementById('modalDate');

    if (authorEl) authorEl.textContent = comment.userName || comment.userFullName || 'Kullanıcı';
    if (taskEl) taskEl.textContent = comment.taskTitle || 'Görev Bilgisi Yok';
    if (contentEl) contentEl.textContent = comment.content || comment.text || '';
    if (dateEl) dateEl.textContent = formatFullDate(comment.createdAt || comment.createdDate);

    // Modalı görünür yap
    const modal = document.getElementById('detailModal');
    if (modal) modal.classList.add('active');
}

// DETAY PENCERESİNİ KAPATMA
function closeDetailModal() {
    const modal = document.getElementById('detailModal');
    if (modal) modal.classList.remove('active');
}

// DIŞARIYA TIKLANDIĞINDA KAPATMA
function closeModalOnOutsideClick(event) {
    if (event.target.classList.contains('modal-overlay')) {
        closeDetailModal();
    }
}

// TARİH BİÇİMLENDİRME (02.08.2026 14:30:00)
function formatFullDate(dateStr) {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return dateStr;

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    return `${day}.${month}.${year} ${hours}:${minutes}:${seconds}`;
}

// 5. YORUM SİLME
function deleteComment(commentId) {
    selectedCommentId = commentId;
    document.getElementById("deleteModal").style.display = "flex";
}

function closeDeleteModal() {
    document.getElementById("deleteModal").style.display = "none";
    document.getElementById("deleteMessage").innerText = "";
    selectedCommentId = null;
}

async function confirmDelete() {
    if (!selectedCommentId)
        return;

    try {

        const response = await fetch(
            `${BASE_URL}/api/comments/${selectedCommentId}`,
            {
                method: "DELETE",
                headers: {
                    "Authorization": "Bearer " + localStorage.getItem("authToken"),
                    "Content-Type": "application/json"
                }
            });

        const message = document.getElementById("deleteMessage");

        if (response.ok) {

            message.style.color = "green";
            message.innerText = "Yorum başarıyla silindi.";

            setTimeout(async () => {
                closeDeleteModal();
                await loadAllDataFromDB();
            }, 1000);

        } else {
            const error = await response.text();
            message.style.color = "red";
            message.innerText = error;
        }
    }
    catch (err) {
        console.log(err);
        document.getElementById("deleteMessage").style.color = "red";
        document.getElementById("deleteMessage").innerText = "Sunucuya bağlanılamadı.";
    }
}

// YARDIMCI FONKSİYONLAR
function renderMessage(msg) {
    const container = document.getElementById('commentsContainer');
    if (container) {
        container.innerHTML = `<p style="text-align:center; color: var(--text-secondary); padding: 20px;">${msg}</p>`;
    }
}

function formatDate(dateStr) {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return dateStr;
    return date.toLocaleDateString('tr-TR', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' });
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

function openAddModal() {
    // Görevler listesini modal içindeki dropdown'a aktar
    const addTaskSelect = document.getElementById('addTaskSelect');
    if (addTaskSelect) {
        addTaskSelect.innerHTML = '<option value="">Görev Seçiniz...</option>';
        const mainSelect = document.getElementById('taskFilter');
        if (mainSelect) {
            Array.from(mainSelect.options).forEach(opt => {
                if (opt.value) { // Boş seçeneği atla
                    const newOpt = document.createElement('option');
                    newOpt.value = opt.value;
                    newOpt.textContent = opt.textContent;
                    addTaskSelect.appendChild(newOpt);
                }
            });
        }
    }

    // Formu temizle
    const form = document.getElementById('addCommentForm');
    if (form) form.reset();

    // Modalı Göster
    const modal = document.getElementById('addModal');
    if (modal) modal.classList.add('active');
}

function closeAddModal() {
    const modal = document.getElementById('addModal');
    if (modal) modal.classList.remove('active');
}

function closeAddModalOnOutsideClick(event) {
    if (event.target.classList.contains('modal-overlay')) {
        closeAddModal();
    }
}

// BACKEND'E YENİ YORUM KAYDETME (POST /api/comments)
async function saveNewComment() {

    const taskId = document.getElementById('addTaskSelect').value;

    const content = document.getElementById('addContentTextarea').value;


    const payload = {

        taskId: taskId,

        content: content

    };

    try {
        const response = await fetch(`${BASE_URL}/api/tasks/${taskId}/comments`, {
            method: 'POST',
            headers: {
                 "Authorization": "Bearer " + localStorage.getItem("authToken"),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            closeAddModal();
            // Verileri veritabanından tekrar çekip ekranı yenile
            if (typeof loadAllDataFromDB === 'function') {
                await loadAllDataFromDB();
            }
        } else {
            alert('Yorum eklenirken bir hata oluştu.');
        }
    } catch (error) {
        console.error('Ekleme hatası:', error);
        alert('Sunucuyla bağlantı kurulamadı.');
    }
}

function toggleSidebar() {
    const sidebar = document.getElementById("mySidebar");

    if (sidebar.style.width === "250px") {
        sidebar.style.width = "0";
    } else {
        sidebar.style.width = "250px";
    }
}


function logout() {
    localStorage.removeItem("token");
    window.location.href = "../../Login/index.html";
}
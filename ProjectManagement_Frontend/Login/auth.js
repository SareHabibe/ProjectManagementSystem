document.getElementById('loginForm').addEventListener("submit", async function(e) {
    e.preventDefault();
    const email= document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch("https://localhost:7075/api/Auth/login",{
            method: "POST",
            headers:{
                "Content-Type": "application/json"
                },
                body:JSON.stringify({
                    email: email,
                    password: password
                })
            });

            if (response.ok) {
                const result = await response.json();
                const token = result.token ? result.token : result.data.token;
                localStorage.setItem("authToken", token);
                alert("Giriş başarılı! Ana sayfaya yönlendiriliyorsunuz.");
                window.location.href="../MainWindow/MainWindow.html";
                
            }
            else{
                alert("E-posta veya şifre hatalı!");
            }
        } catch (error){
            console.error("Hata oluştu.",error);
            alert("Backend sistemine bağlanılamadı. Projenin açık olduğundan emin olun!");
        }
    });
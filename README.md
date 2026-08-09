# ProjectManagementSystem

Proje Yönetim Sistemi

Bu proje, ASP.NET Core Web API kullanılarak geliştirilmiş bir Proje Yönetim Sistemi uygulamasıdır.

-Kullanılan Teknolojiler-


•Backend = C#, Asp.Net Core Web API, Entity Framework Core
•Veri Erişimi = Entity Framework Core (ORM) (Migration desteği)
•Veritabanı = Microsoft SQL Server
•Kimlik Doğrulama = JWT Bearer Authentication
•Frontend = HTML, CSS, JAVASCRIPT
•Mimari = Katmanlı Mimari, Repository Pattern, Service Layer
•Dökümantasyon = Swagger/OpenAPI entegrasyonu mevcuttur.
•Veri Transferi = Entity modelleri doğrudan API çıktısı olarak kullanılmamış, DTO  yaklaşımı tercih edilmiştir.


-Özellikler-


•Kullanıcı kayıt ve giriş işlemleri
•JWT tabanlı kimlik doğrulama 
•Rol bazlı ve proje üyeliğine dayalı yetkilendirmeler (Admin, ProjectManager, TeamMember, Viewer)
•Proje Yönetimi (Oluşturma, güncelleme, listeleme, arşivleme)
•Proje Üye Yönetimi (Proje üyelerini listeleme, üye ekleme, üye çıkarma işlemleri)
•Görev Yönetimi (Görevleri oluşturma, listeleme, düzenleme, silme (soft delete) işlemleri)
•Yorum Sistemi (Yetkili proje üyelerinin görevlere yorum yazabilmesi)
•Görev Geçmişi (Task History - Durum, atama ve öncelik değişim logları)
•Zaman Kaydı (Timelogs - Görev için harcanan çalışma saatlerinin girilmesi)
•Filtreleme, Sıralama ve Sayfalama (Pagination)
•Soft Delete (Verilerin kalıcı silinmesi yerine IsDeleted ile saklanması)
•Global Exception Handling (Hata Yönetimi)
•Swagger/OpenAPI üzerinden interaktif test imkanı





-ÖRNEK VERİLER (DataSeeder)-

Uygulama ilk kez açıldığında "DataSeeder" mekanizması otomatik olarak çalışır ve test edebilmeniz için örnek verileri veritabanına basar. Aşağıdaki test hesaplarıyla giriş yapabilirsiniz.

| Rol | E-posta | Şifre |
Admin | `admin@project.com` | `Admin123!`|
Project Manager | `projectManager@project.com` | `Pm123!`|
Team Member| `member@project.com` | `member123!`|
Viewer| `viewer@project.com` | `Viewer123!` |




---KURULUM VE ÇALIŞTIRMA---


--GEREKSİNMLER--

Projeyi çalıştırmadan önce bilgisayarınızda aşağıdakilerin kurulu olması gerekmektedir:

-Visual Studio Community 2022

-VS Code (Frontend için)

- .NET 8.0 SDK

-SQL Server Express (Veritabanı motoru için)

-SQL Server Management Studio (SSMS) (Veritabanını yönetmek/görmek için isteğe bağlı arayüz)





---BACKEND API VE VERİTABANI KURULUMU---

Projeyi kendi bilgisayarınızda ayağa kaldırmak için sırasıyla şu adımları izleyin:

1. Projeyi İndirin ve Açın
Projeyi GitHub'dan ZIP olarak indirin ve bir klasöre Tümünü Ayıkla diyerek çıkartın.
Çıkarttığınız klasörün içindeki .sln (Solution) uzantılı dosyaya çift tıklayarak projeyi Visual Studio ile açın.



2. .NET 8.0 SDK ve SQL Server Kurulumu
Bilgisayarınızda .NET 8.0 SDK'nın ve SQL Server Express sürümünün kurulu olduğundan emin olun.
NuGet paketleri projeyi açtığınızda otomatik olarak yüklenecektir; ek bir komut girmenize gerek yoktur.



3. Backend Kısmında Veritabanı Bağlantı Ayarlarını Yapın (appsettings.json)
Proje içerisindeki appsettings.json dosyasını açın.
ConnectionStrings alanındaki sunucu adını kendi yerel SQL Server sunucu adınıza göre güncelleyin 

Örnek bağlantı cümlesi:
"Server=localhost\\SQLEXPRESS;Database=ProjectManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"


4. Veritabanını Oluşturma (Migration)
Proje ilk kez çalıştırıldığında veya Entity Framework Core altyapısı sayesinde veritabanı ve tablolar otomatik olarak SQL Server üzerinde oluşturulacaktır.


5. Projeyi Çalıştırın
Visual Studio üzerinde üst menüde yer alan yeşil Start butonuna basarak projeyi çalıştırabilirsiniz.





---FRONTEND KISMINI ÇALIŞTIRMA---

VS Code uygulamasını açın.

File > Open Folder seçeneğine tıklayın ve ProjectManagement_Frontend yazan klasörünüzü seçip açın.

Üst kısımdan kullanmak istediğiniz tarayıcıyı seçip Start butonuna basarak arayüzü çalıştırın.




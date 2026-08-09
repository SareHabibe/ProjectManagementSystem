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
• Global Exception Handling (Hata Yönetimi)
•Swagger/OpenAPI üzerinden interaktif test imkanı


-Örnek Veriler (DataSeeder)-

Uygulama ilk kez açıldığında "DataSeeder" mekanizması otomatik olarak çalışır ve test edebilmeniz için örnek verileri veritabanına basar. Aşağıdaki test hesaplarıyla giriş yapabilirsiniz.

| Rol | E-posta | Şifre |
Admin | `admin@project.com` | `Admin123!`|
Project Manager | `projectManager@project.com` | `Pm123!`|
Team Member| `member@project.com` | `member123!`|
Viewer| `viewer@project.com` | `Viewer123!` |


-Kurulum ve Çalıştırma-

Projeyi GitHub üzerinden klonlayarak kendi local ortamınızda çalıştırmak için sırasıyla şu adımları izleyebilirsiniz:

1. -Projeyi Klonlayın-

   Terminal veya komut satırınızı açarak projeyi bilgisayarınıza indirin ve proje dizinine gidin:

   ```bash
   git clone <repository-url>
   cd ProjectManagementSystem

2. -Veri Tabanı Bağlantısını Yapılandırın-

   Proje klasöründeki appsettings.json dosyasını açın. Kendi bilgisayarınızdaki SQL Server sunucu adresine göre DefaultConnection alanını güncelleyin.

3. -Veritabanını ve Migration İşlemini Gerçekleştirin-

   Terminalde proje klasörünün içindeyken, veritabanı tablolarını ve örnek verileri otomatik olarak oluşturmak için konsola şu komutu yazın:

    1. dotnet ef database update

    Projeyi başlatın:
    
    2. dotnet run
    



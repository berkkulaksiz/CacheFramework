Sample Cache API Kullanım Kılavuzu
Bu belge, Sample.Cache.Api projesi için kullanım kılavuzudur. Bu örnek API, MicroFrame Cache Framework'ün kapsamlı özelliklerini demonstre etmek için geliştirilmiştir.

API Hakkında
Bu API, basit bir e-ticaret sisteminin ürün ve kategori yönetimini simüle etmektedir. Ürünler ve kategoriler üzerinde CRUD işlemleri yapabilir ve çeşitli cache stratejileri test edebilirsiniz.

Başlangıç
Projeyi çalıştırmak için:

bash
cd Sample.Cache.Api
dotnet run
API varsayılan olarak https://localhost:5001 ve http://localhost:5000 adreslerinde çalışacaktır.

Tarayıcınızda https://localhost:5001 adresine gittiğinizde, Swagger UI'ı görüntüleyebilirsiniz.

Endpoint'ler
Ürünler
GET /api/products - Tüm ürünleri getirir (basit cache, 60 saniye)
GET /api/products/{id} - ID'ye göre ürün getirir (gelişmiş cache, 300 saniye)
GET /api/products/category/{categoryId} - Kategoriye göre ürünleri getirir (özel strateji, 300 saniye)
GET /api/products/featured - Öne çıkan ürünleri getirir (yüksek performans cache, 600 saniye)
POST /api/products - Yeni ürün ekler (ilgili cache'leri invalidate eder)
PUT /api/products/{id} - Ürün günceller (ilgili cache'leri invalidate eder)
DELETE /api/products/{id} - Ürün siler (ilgili cache'leri invalidate eder)
Kategoriler
GET /api/categories - Tüm kategorileri getirir (basit cache, 120 saniye)
GET /api/categories/{id} - ID'ye göre kategori getirir (özel strateji, 300 saniye)
GET /api/categories/popular - Popüler kategorileri getirir (yüksek performans cache, 600 saniye)
POST /api/categories - Yeni kategori ekler (ilgili cache'leri invalidate eder)
PUT /api/categories/{id} - Kategori günceller (ilgili cache'leri invalidate eder)
DELETE /api/categories/{id} - Kategori siler (ilgili cache'leri invalidate eder)
Metrikler
GET /api/metrics - Cache metriklerini getirir (hit rate, ortalama gecikme)
POST /api/metrics/reset - Cache metriklerini sıfırlar
POST /api/metrics/clear - Tüm cache'lenmiş öğeleri temizler
Cache Politikaları ve Stratejileri
Bu örnek API'de farklı cache politikaları ve stratejileri kullanılmıştır:

Basit Cache: Temel cache işlevi, belirtilen süre boyunca sonuçları saklar
csharp
[Cached(60)]
API Politikası: API yanıtları için optimize edilmiş cache davranışı
csharp
[Cached(300, CachePolicy.ApiPolicy)]
Yüksek Performans Politikası: Stale-while-revalidate ve sıkıştırma özellikleri ile yüksek performans
csharp
[Cached(600, CachePolicy.HighPerformancePolicy)]
Özel Stratejiler: ProductCacheStrategy ve CategoryCacheStrategy ile özelleştirilmiş cache davranışı
csharp
[Cached(300, typeof(ProductCacheStrategy))]
Cache Davranışını Test Etme
Cache davranışını test etmek için:

Önce bir GET isteği yapın ve cevap süresini not edin
Aynı GET isteğini tekrarlayın, cevap süresi dramatik olarak azalmalıdır (cache hit)
GET /api/metrics çağırarak cache hit rate'ini kontrol edin
PUT/POST/DELETE işlemi yapın ve ilgili cache'in invalidate edildiğini görmek için GET isteğini tekrarlayın
POST /api/metrics/clear kullanarak tüm cache'i temizleyin ve sonuçları karşılaştırın
Redis Entegrasyonu
Bu örnek uygulama, Redis olmadan da çalışabilir. Redis entegrasyonunu aktifleştirmek için:

Redis sunucusunu yerel veya uzak makinede çalıştırın
Program.cs dosyasında şu satırı uncomment edin:
csharp
services.AddRedisCaching("localhost:6379,abortConnect=false");
Metrik Görselleştirme
Cache metriklerini görselleştirmek için:

GET /api/metrics endpoint'ini tekrarlayan şekilde çağırarak metrik verilerini alın
Hit rate ve gecikme süresi verilerini not edin
Çeşitli senaryoları test ettikten sonra metrikleri karşılaştırın
Önemli Notlar
Bu demo uygulaması, tüm verileri bellek içinde (in-memory) tutar ve uygulama yeniden başlatıldığında veriler sıfırlanır
Örnek veri, uygulama başlatıldığında otomatik olarak yüklenir
Cache stratejileri, gerçek dünya senaryolarını simüle etmek üzere tasarlanmıştır
Gerçek uygulama senaryolarında, invalidasyon mantığını kendi iş ihtiyaçlarınıza göre özelleştirmelisiniz
İleri Düzey Özellikler
Daha ileri düzey özellikleri test etmek için:

CategoryCacheStrategy ve ProductCacheStrategy kodlarını inceleyin ve değiştirin
İlişkisel invalidasyon mantığını gözlemleyin (bir kategori güncellendiğinde, o kategorideki ürünlerin cache'i de invalidate edilir)
Circuit Breaker davranışını test etmek için Redis'i kapatıp açın
Cache key generation ve timeout özelleştirmesi için kod örneklerini inceleyin
Cache Key İnceleme
API'ın ürettiği cache key'leri incelemek için:

Bir isteğin nasıl cache'lendiğini görmek için herhangi bir GET isteği yapın
Debug seviyesindeki logları inceleyerek oluşturulan cache key'leri görün
Farklı endpoint'ler için farklı key stratejilerini gözlemleyin
Yük Testi
Cache'in performans etkisini test etmek için:

Cache olmadan API'a yük testi yapın
Cache ile API'a aynı yük testini yapın
Metrikleri karşılaştırarak performans kazanımını ölçün
Yük testi için Apache Benchmark veya wrk gibi araçları kullanabilirsiniz.

Geliştirme
Bu örnek projeyi kendi ihtiyaçlarınıza göre geliştirmek için:

Yeni controller'lar ve servisler ekleyin
Özel cache stratejileri oluşturun
Cache key generation ve timeout özelleştirmesi yapın
İlişkisel invalidasyon mantığını genişletin
Sorun Giderme
Yaygın sorunlar ve çözümleri:

Redis bağlantı hatası: Redis sunucusunun çalıştığından ve bağlantı bilgilerinin doğru olduğundan emin olun
Cache hit olmama: Cache key üretim mantığını ve invalidasyon stratejisini kontrol edin
Metrik sıfırlanması: Uygulama yeniden başlatıldığında metrikler sıfırlanır
Performans sorunları: Cache sürelerini düşürün veya yüksek performans politikası kullanın
Kaynaklar
MicroFrame Caching Framework Dokümantasyonu
Redis Dokümantasyonu
ASP.NET Core Caching Dokümantasyonu
Circuit Breaker Pattern
İletişim
Bu örnek proje veya caching framework hakkında sorularınız için:

Email: support@microframe.com
GitHub: https://github.com/microframe/caching

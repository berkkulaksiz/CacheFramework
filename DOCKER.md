Docker İle Çalıştırma Talimatları
Bu belge, Sample Cache API projesini Docker ortamında çalıştırmak için gerekli talimatları içerir.

Önkoşullar
Docker Engine ve Docker Compose yüklü olmalıdır
.NET SDK 8.0 (geliştirme için)
SSL Sertifikası Oluşturma
HTTPS için geçerli bir sertifika oluşturmak için aşağıdaki adımları izleyin:

bash
# SSL geliştirme sertifikası oluşturma (Windows)
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p password123
dotnet dev-certs https --trust

# SSL geliştirme sertifikası oluşturma (macOS/Linux)
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p password123
dotnet dev-certs https --trust
Docker Image Oluşturma ve Çalıştırma
Temel Docker Compose ile
bash
# Docker image'larını oluşturun
docker-compose build

# Servisleri başlatın
docker-compose up -d

# Logları izleyin
docker-compose logs -f
Tam Docker Compose ile (Redis Insight dahil)
bash
# Tam sürümü çalıştırın
docker-compose -f docker-compose.full.yml up -d

# Logları izleyin
docker-compose -f docker-compose.full.yml logs -f
Erişim Noktaları
Servisler başlatıldıktan sonra aşağıdaki URL'lerden erişebilirsiniz:

Sample Cache API:
HTTP: http://localhost:5000
HTTPS: https://localhost:5001
Swagger: http://localhost:5000/index.html
Redis Insight (Redis yönetim arayüzü):
http://localhost:8001
İlk bağlantıda Redis sunucusu için "redis:6379" adresini kullanın
Docker Services
sample.cache.api
ASP.NET Core tabanlı web API servisimiz. Bu servis, çeşitli cache stratejilerini demonstre eder.

Port: 5000 (HTTP), 5001 (HTTPS)
Environment: Development
Dependencies: Redis
redis
Redis önbellek sunucusu.

Port: 6379
Persistent Storage: redis-data volume
Configuration: AOF (Append Only File) aktivedir
redis-insight (İsteğe Bağlı)
Redis yönetimi için görsel arayüz.

Port: 8001
Dependencies: Redis
Değişkenler
Docker Compose dosyasında aşağıdaki ortam değişkenlerini özelleştirebilirsiniz:

ASPNETCORE_ENVIRONMENT: Uygulama ortamı
Caching__DefaultTimeToLiveSeconds: Varsayılan cache süresi
Redis__ConnectionString: Redis bağlantı bilgileri
Sorun Giderme
Redis Bağlantı Sorunları
bash
# Redis CLI ile Redis'e bağlanın
docker exec -it redis-cache redis-cli

# PING komutu ile çalışıp çalışmadığını kontrol edin
PING

# Cache keylerini listeleyin
KEYS *
API Sorunları
bash
# API konteynerindeki logları görüntüleyin
docker logs sample-cache-api

# API konteynerine bir shell açın
docker exec -it sample-cache-api /bin/bash

# API servisini yeniden başlatın
docker-compose restart sample.cache.api
Temizlik
bash
# Servisleri durdurun
docker-compose down

# Volumeleri de dahil ederek servisleri durdurun
docker-compose down -v

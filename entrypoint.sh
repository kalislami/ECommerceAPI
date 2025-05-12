#!/bin/bash

# Menunggu SQL Server agar siap
echo "Menunggu SQL Server agar siap..."
sleep 15  # Sesuaikan jika perlu untuk lebih lama tergantung pada kecepatan SQL Server

# Menjalankan migrasi database
echo "Menjalankan migrasi database..."
dotnet ef database update --no-build --project ECommerceApi.csproj --startup-project ECommerceApi.csproj

# Menjalankan aplikasi
echo "Menjalankan aplikasi..."
dotnet ECommerceApi.dll

#  Pastikan file `entrypoint.sh` menggunakan format **LF** (Linux line endings), bukan **CRLF**.
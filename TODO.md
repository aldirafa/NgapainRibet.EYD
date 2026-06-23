# NgapainRibet.EYD

Pemeriksa tata bahasa dan ejaan Bahasa Indonesia terinspirasi dari Grammarly.

## `Core`

- Pastikan semua public API terdokumentasi XML dengan jelas.
- Siapkan Core agar bisa di-export ke NuGet. Boleh tanyakan dulu biodata atau info lain yang perlu disiapkan untuk NuGet Export
    - Ini termasuk workflow YAML di GitHub Actions dan Trusted apa gitu lupa di NuGetnya.

## KBBI

- [ ] Tambahkan fitur untuk update database SQLite. Ini akan berguna jika kata baku yang baru muncul dan belum masuk KBBI, dan juga untuk bahasa daerah yang dibakukan penggunaannya (karena user bisa dari berbagai latar belakang kultural di Indonesia).
    - [ ] Tambah kata baru.
    - [ ] Revisi/ubah kata dari database.
    - [ ] Hapus kata dari database.
- [ ] Atau, ketimbangkan update database ini, tambahkan fitur untuk custom database yang di-load bersandingan dengan SQLite yang sudah ada.

## Pemeriksa Ejaan

:ok:

## EYD

- [ ] `Core` -> tambahkan fitur pemeriksa kalimat. Aturan-aturan EYD dikembangkan menjadi function, class, atau apa gitu sehingga kita bisa memeriksa penggunaan titik, koma, dsb. dengan menggunakan library ini.
    - [ ] Masing-masing entry di database kata perlu di-specify juga kelas katanya (nomina, verba, etc.) karena akan digunakan di sini. **TODO ALDI: Bikin rangkuman kelas kata**
    - [ ] Pelajari sistem token untuk membantu melakukan pemeriksaan kalimat. Siapa tahu?

Implementasi pemeriksa kalimat kita lakukan bertahap agar tidak overwhelmed. 

- **Penggunaan Huruf dan Penulisan Kata**
- [ ] Stage 0: Memastikan kata yang ditulis masuk KBBI. Kata yang tidak masuk KBBI ditawarkan masuk kamus Custom. Kata yang ditulis semua huruf kapital tidak perlu dicek ejaannya. Kata yang diawali huruf kapital tidak perlu dicek ejaannya.
- [ ] Stage 1: Penggunaan huruf kapital.
- [ ] Stage 2: Penggunaan kata turunan.
- [ ] Stage 3: Pemenggalan kata.
- [ ] Stage 4: Penulisan kata depan.
- [ ] Stage 5: Penulisan partikel.
- [ ] Stage 6: Kamus singkatan: **TODO ALDI: Cari database kata singkatan**
- [ ] Stage 7: Penulisan angka.
- [ ] Stage 8: Kata ganti ku-, kau-, -ku, -mu, dan -nya
- [ ] Stage 9: Kata Sandang si dan sang
- [ ] Stage 10: Penggunaan huruf miring dan tebal dalam markdown dan HTML.

- **Penggunaan Tanda Baca**
- [ ] Stage 0: Prerequisite: deteksi kata, frasa, klausa, kalimat
- [ ] Stage 1: Tanda titik
- [ ] Stage 2: Tanda koma
- [ ] Stage 3: Tanda titik koma
- [ ] Stage 4: Tanda titik dua
- [ ] Stage 5: Tanda hubung
- [ ] Stage 6: Tanda pisah
- [ ] Stage 7: Tanda tanya
- [ ] Stage 8: Tanda seru
- [ ] Stage 9: Tanda elipsis
- [ ] Stage 10: Tanda petik
- [ ] Stage 11: Tanda petik tunggal
- [ ] Stage 12: Tanda kurung
- [ ] Stage 13: Tanda kurung siku
- [ ] Stage 14: Tanda garis miring
- [ ] Stage 15: Tanda apostrof
- [ ] Stage 16: Deteksi bibliografi dan mengecualikan bibliografi dari pemeriksaan EYD
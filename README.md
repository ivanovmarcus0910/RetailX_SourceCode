# RetailX – Hệ thống quản lý bán hàng & nhân sự đa chi nhánh (Multi-tenant)

**RetailX** là hệ thống web hỗ trợ **quản lý bán hàng, kho, nhân sự và báo cáo** cho các cửa hàng bán lẻ, chuỗi cửa hàng, SME.  
Mỗi công ty (tenant) có **database riêng**, nhưng dùng chung một hệ thống trung tâm.

> Tương tự mô hình như KiotViet / các phần mềm POS, nhưng được xây bằng **ASP.NET Core MVC** với kiến trúc phân lớp (DAO – Repository – Service).

---

## 🎯 Mục tiêu hệ thống

- Giúp **chủ doanh nghiệp** quản lý tập trung: doanh thu, tồn kho, nhân sự, chi nhánh.
- Tối ưu quy trình **bán hàng tại quầy** (POS), nhập – xuất – chuyển kho.
- Hỗ trợ mô hình **multi-tenant**: một hệ thống, nhiều công ty, mỗi công ty có DB riêng.

---

## 👥 Vai trò & phân quyền

- **System Admin (Hệ thống)**
  - Quản lý Tenant (công ty sử dụng hệ thống).
  - Khóa/mở Tenant, hỗ trợ kỹ thuật.

- **Owner (Chủ cửa hàng / Chủ doanh nghiệp) – trong từng Tenant**
  - Cấu hình thông tin công ty, chi nhánh.
  - Quản lý nhân sự (thêm Staff, phân quyền).
  - Xem báo cáo tổng quan: doanh thu, lợi nhuận, tồn kho.

- **Manager / Staff (Quản lý / Nhân viên)**
  - Quản lý sản phẩm, danh mục, nhà cung cấp.
  - Quản lý nhập hàng, xuất hàng, điều chỉnh kho.
  - Quản lý đơn hàng, khách hàng, chương trình khuyến mãi.

- **Cashier (Thu ngân)**
  - Bán hàng tại quầy (POS).
  - Tạo hóa đơn, áp dụng giảm giá, in bill.
  - Xem lịch sử hóa đơn mình đã tạo.

---

## ✨ Tính năng chính

### 1. Quản lý sản phẩm & danh mục
- Danh mục sản phẩm, thương hiệu, đơn vị tính.
- Sản phẩm với mã SKU, giá vốn, giá bán, tồn kho.
- Hỗ trợ barcode (tùy chỉnh).

### 2. Quản lý kho & nhập xuất
- Nhập hàng từ nhà cung cấp.
- Xuất hàng bán lẻ / điều chuyển.
- Điều chỉnh tồn kho (kiểm kho).
- Lịch sử phiếu nhập/xuất, tồn kho theo chi nhánh.

### 3. Bán hàng (POS)
- Giao diện bán hàng nhanh cho thu ngân.
- Tìm sản phẩm theo tên / mã / barcode.
- Tính tiền, giảm giá, thu tiền, in hóa đơn.
- Xem lại hóa đơn, hoàn/đổi hàng (tùy cấu hình).

### 4. Quản lý khách hàng & nhà cung cấp
- Danh sách khách hàng, lịch sử mua.
- Danh sách nhà cung cấp, lịch sử nhập hàng.
- Thông tin liên hệ, ghi chú.

### 5. Quản lý nhân sự & ca làm
- Danh sách nhân viên theo Tenant.
- Phân quyền theo role (Owner / Manager / Staff / Cashier).
- (Optional) Quản lý ca làm việc, lương thưởng.

### 6. Báo cáo
- Báo cáo doanh thu theo ngày/tháng/chi nhánh.
- Báo cáo bán hàng theo sản phẩm/nhóm hàng.
- Báo cáo tồn kho, hàng sắp hết / quá tồn.

---

## 🧩 Kiến trúc & Multi-tenant

### Kiến trúc phân lớp

Dự án áp dụng mô hình:

- **Database** – SQL Server.
- **DAO (Data Access Object)** – Làm việc trực tiếp với DbContext/Entity.
- **Repository** – Đóng gói thao tác dữ liệu, tách khỏi business logic.
- **Service (Business)** – Chứa nghiệp vụ (tính tồn kho, tính doanh thu, v.v.).
- **MVC (UI)** – ASP.NET Core MVC, Controller + View + ViewModel.

Cấu trúc solution (ví dụ):

```text
RetailX.sln
 ├─ BusinessObject/              (Entity, ViewModel dùng chung)
 ├─ DataAccessObject/            (DbContext cho từng Tenant)
 ├─ Repositories/                (Interface + Implement Repository)
 ├─ Services/                    (Business logic)
 └─ RetailXMVC/                  (Web MVC project – UI, Controllers, Views)

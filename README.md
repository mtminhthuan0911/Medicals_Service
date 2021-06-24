# Clinic Service System
Hệ thống xây dựng nhằm mục đích nâng cao sự tương tác giữa phòng khám, các bác sĩ với các bệnh nhân, khách hàng thông qua nền tảng Web và Mobile.

## Clone source code
```
git clone https://gitlab.com/hungvuongwork/clinic-service-system.git
```
> Lưu ý: Nếu git yêu cầu đăng nhập, mấy đứa vào `Settings/Password` của GitLab để update mật khẩu nha.

## Thành viên
- Vương Hùng
- Thái Khoa
- Thuận

## Tổng quan
1. Đối với khách hàng, bệnh nhân đang có nhu cầu khám chữa bệnh:
	- Được phép xem các gói dịch vụ phù hợp với nhu cầu.
	- Đăng ký khám, chữa bệnh trực tuyến.
	- Tra cứu các triệu chứng đang gặp phải và từ đó hệ thống sẽ gợi ý gói dịch vụ phù hợp cho đối tượng.
	- Tra cứu lịch sử kết quả khám, chữa bệnh tại phòng khám.
2. Đối với các bác sĩ:
	- Ghi nhận lại kết quả - tình trạng khám bệnh, chữa bệnh của bệnh nhân.
	- Kê toa đơn thuốc dựa trên kết quả khám, chữa bệnh.
	- Tra cứu lịch sử khám, chữa bệnh các bệnh nhân.
3. Đối với quản trị viên:
	- Quản lý nhân sự của phòng khám
	- Phân quyền người dùng
	- Xem thống kê, báo cáo.

## Câu lệnh upload source lên GitLab
```
git add .
git commit -m "dòng miêu tả"
git push
```

## Tham khảo
1. Nguyên tắc thiết kế API:
	- https://blog.codegym.vn/2020/01/31/nguyen-tac-thiet-ke-api/
	- https://itplusx.info/restful-api-thiet-ke-phan-2/
2. Tuỳ chọn fields trả về:
	- https://www.c-sharpcorner.com/uploadfile/f7a3ed/fields-filtering-in-asp-net-web-api/
3. Http status return:
	- https://www.moesif.com/blog/technical/api-design/Which-HTTP-Status-Code-To-Use-For-Every-CRUD-App/
4. Automapper edit:
	- https://stackoverflow.com/questions/8844443/using-automapper-in-the-edit-action-method-in-an-mvc3-application
5. Lấy thông tin ảo:
	- https://www.fakeaddressgenerator.com/usa_address_generator
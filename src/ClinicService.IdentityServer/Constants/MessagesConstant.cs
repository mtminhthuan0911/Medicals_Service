using System;
namespace ClinicService.IdentityServer.Constants
{
    public static class MessagesConstant
    {
        public const string RECORD_REQUIRED = "{0} bắt buộc nhập.";
        public const string RECORD_NOT_FOUND = "{0} không tìm thấy.";
        public const string RECORD_MAX_LENGTH = "{0} chứa tối đa {1} ký tự.";
        public const string RECORD_NOT_REMOVE = "{0} này không thể xoá.";

        public const string DEFAULT_NOT_FOUND = "Không tìm thấy dữ liệu";
        public const string DEFAULT_BAD_REQUEST = "Đã có lỗi xảy ra.";
    }
}

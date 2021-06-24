// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using ClinicService.IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using ClinicService.IdentityServer.Data.Entities;
using System.Collections.Generic;

namespace ClinicService.IdentityServer.Data
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    string adminRoleName = "Admin";
                    string doctorRoleName = "Doctor";
                    string patientRoleName = "Patient";

                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    #region Quyền
                    if (!roleManager.Roles.Any())
                    {
                        roleManager.CreateAsync(new IdentityRole
                        {
                            Id = "ROLE_" + adminRoleName.ToUpper(),
                            Name = adminRoleName,
                            NormalizedName = adminRoleName.ToUpper(),
                        });

                        roleManager.CreateAsync(new IdentityRole
                        {
                            Id = "ROLE_" + doctorRoleName.ToUpper(),
                            Name = doctorRoleName,
                            NormalizedName = doctorRoleName.ToUpper(),
                        });

                        roleManager.CreateAsync(new IdentityRole
                        {
                            Id = "ROLE_" + patientRoleName.ToUpper(),
                            Name = patientRoleName,
                            NormalizedName = patientRoleName.ToUpper(),
                        });
                    }
                    #endregion Quyền

                    #region Người dùng
                    if (!userManager.Users.Any())
                    {
                        var addedAdminResult = userManager.CreateAsync(new ApplicationUser
                        {
                            Id = "USER_ADMIN",
                            UserName = "admin",
                            FirstName = "Quản trị viên",
                            LastName = "1",
                            Email = "admin@clinicservice.local",
                            EmailConfirmed = true,
                            CreatedDate = DateTime.Now,
                            LockoutEnabled = false
                        }, "Default@123").Result;
                        if (addedAdminResult.Succeeded)
                        {
                            var user = userManager.FindByNameAsync("admin").Result;
                            userManager.AddToRoleAsync(user, adminRoleName);
                        }

                        var addedDoctorResult = userManager.CreateAsync(new ApplicationUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = "test-doctor",
                            FirstName = "Bác sĩ",
                            LastName = "Test",
                            Email = "test-doctor@clinicservice.local",
                            EmailConfirmed = true,
                            CreatedDate = DateTime.Now,
                            LockoutEnabled = false
                        }, "Default@123").Result;
                        if (addedDoctorResult.Succeeded)
                        {
                            var user = userManager.FindByNameAsync("test-doctor").Result;
                            userManager.AddToRoleAsync(user, doctorRoleName);
                        }

                        var addedPatientResult = userManager.CreateAsync(new ApplicationUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = "test-patient",
                            FirstName = "Bệnh nhân",
                            LastName = "Test",
                            Email = "test-paitent@clinicservice.local",
                            EmailConfirmed = true,
                            CreatedDate = DateTime.Now,
                            LockoutEnabled = false
                        }, "Default@123").Result;
                        if (addedPatientResult.Succeeded)
                        {
                            var user = userManager.FindByNameAsync("test-patient").Result;
                            userManager.AddToRoleAsync(user, patientRoleName);
                        }
                    }
                    #endregion Người dùng

                    #region Lệnh
                    if (!context.Commands.Any())
                    {
                        context.Commands.AddRange(new List<Command>
                        {
                            new Command { Id = "READ", Name = "Đọc" },
                            new Command { Id = "CREATE", Name = "Thêm" },
                            new Command { Id = "UPDATE", Name = "Sửa" },
                            new Command { Id = "DELETE", Name = "Xoá" }
                        });
                        context.SaveChanges();
                    }
                    #endregion Lệnh

                    #region Chức năng
                    if (!context.Functions.Any())
                    {
                        context.Functions.AddRange(new List<Function>
                        {
                            new Function { Id = "DASHBOARD", Name = "Tổng quan", ParentId = null, SortOrder = 1, Url = "/dashboard", Icon = "fa-home" },

                            new Function { Id = "CONTENT", Name = "Nội dung", ParentId = null, SortOrder = 2, Url = "/contents", Icon = "fa-puzzle-piece" },

                            new Function { Id = "CONTENT_WEBSITE_SECTION", Name = "Thành phần Web", ParentId = "CONTENT", SortOrder = 1, Url = "/contents/website-sections", Icon = "fa-cubes" },
                            new Function { Id = "CONTENT_SPECIALTY", Name = "Chuyên khoa", ParentId = "CONTENT", SortOrder = 2, Url = "/contents/specialties", Icon = "fa-list" },
                            new Function { Id = "CONTENT_MEDICAL_SERVICE", Name = "Dịch vụ", ParentId = "CONTENT", SortOrder = 3, Url = "/contents/medical-services", Icon = "fa-ambulance" },
                            new Function { Id = "CONTENT_CLINIC_BRANCH", Name = "Chi nhánh", ParentId = "CONTENT", SortOrder = 4, Url = "/contents/clinic-branches", Icon = "fa-hospital-o" },

                            new Function { Id = "ADMINISTRATION", Name = "Hành chính", ParentId = null, SortOrder = 3, Url = "/adminstrations", Icon = "fa-briefcase" },

                            new Function { Id = "ADMINISTRATION_APPOINTMENT", Name = "Phiếu hẹn khám", ParentId = "ADMINISTRATION", SortOrder = 1, Url = "/administrations/appointments", Icon = "fa-files-o" },
                            new Function { Id = "ADMINISTRATION_REAPPOINTMENT", Name = "Phiếu tái khám", ParentId = "ADMINISTRATION", SortOrder = 2, Url = "/administrations/re-appointments", Icon = "fa-files-o" },
                            new Function { Id = "ADMINISTRATION_MEDICAL_EXAMINATION", Name = "Phiếu khám bệnh", ParentId = "ADMINISTRATION", SortOrder = 3, Url = "/administrations/medical-examinations", Icon = "fa-files-o" },

                            new Function { Id = "PAYMENT", Name = "Thanh toán", ParentId = null, SortOrder = 4, Url = "/payments", Icon = "fa-credit-card" },

                            new Function { Id = "PERSONNEL", Name = "Nhân sự", ParentId = null, SortOrder = 5, Url = "/personnel", Icon = "fa-users" },

                            new Function { Id = "PERSONNEL_DOCTOR", Name = "Bác sĩ", ParentId = "PERSONNEL", SortOrder = 1, Url = "/personnel/doctors", Icon = "fa-user-md" },
                            new Function { Id = "PERSONNEL_PATIENT", Name = "Bệnh nhân", ParentId = "PERSONNEL", SortOrder = 2, Url = "/personnel/patients", Icon = "fa-user" },

                            new Function { Id = "STATISTIC", Name = "Thống kê", ParentId = null, SortOrder = 6, Url = "/statistic", Icon = "fa-bar-chart" },

                            new Function { Id = "STATISTIC_MONTHLY_NEWMEMBER", Name = "Đăng ký từng tháng", ParentId = "STATISTIC", SortOrder = 1, Url = "/statistic/monthly-registers", Icon = "fa-bar-chart"},
                            new Function { Id = "STATISTIC_MONTHLY_APPOINTMENT", Name = "Số phiếu hẹn từng tháng", ParentId = "STATISTIC", SortOrder = 2, Url = "/statistic/monthly-appointments", Icon = "fa-bar-chart"},
                            new Function { Id = "STATISTIC_MONTHLY_PERSONNEL", Name = "Nhân sự từng tháng", ParentId = "STATISTIC", SortOrder = 3, Url = "/statistic/monthly-personnel", Icon = "fa-bar-chart" },

                            new Function { Id = "CATEGORY", Name = "Danh mục", ParentId = null, SortOrder = 7, Url = "/categories", Icon="fa-list" },

                            new Function { Id = "CATEGORY_STATUS_CATEGORY", Name = "Danh mục tình trạng", ParentId = "CATEGORY", SortOrder = 1, Url = "/categories/status-categories", Icon = "fa-bookmark" },
                            new Function { Id = "CATEGORY_PAYMENT_METHOD", Name = "Phương thức thanh toán", ParentId = "CATEGORY", SortOrder = 2, Url = "/categories/payment-methods", Icon = "fa-credit-card" },
                            new Function { Id = "CATEGORY_MEDICAL_SERVICE_TYPE", Name = "Danh mục dịch vụ", ParentId = "CATEGORY", SortOrder = 3, Url = "/categories/medical-service-types", Icon = "fa-list" },

                            new Function { Id = "SYSTEM", Name = "Hệ thống", ParentId = null, SortOrder = 8, Url = "/systems", Icon="fa-cogs" },

                            new Function { Id = "SYSTEM_USER", Name = "Người dùng", ParentId = "SYSTEM", SortOrder = 1, Url = "/systems/users", Icon="fa-users" },
                            new Function { Id = "SYSTEM_ROLE", Name = "Nhóm vai trò", ParentId = "SYSTEM", SortOrder = 2, Url = "/systems/roles", Icon="fa-user-circle" },
                            new Function { Id = "SYSTEM_FUNCTION", Name = "Chức năng", ParentId = "SYSTEM", SortOrder = 3, Url = "/systems/functions", Icon="fa-cube" },
                            new Function { Id = "SYSTEM_PERMISSION", Name = "Nhóm quyền", ParentId = "SYSTEM", SortOrder = 4, Url = "/systems/permissions", Icon="fa-hand-paper-o" },
                        });
                        context.SaveChanges();
                    }
                    #endregion Chức năng

                    #region Thêm các lệnh cho chức năng & phân quyền
                    var functions = context.Functions;

                    if (!context.FunctionCommands.Any())
                    {
                        foreach (var function in functions)
                        {
                            context.FunctionCommands.Add(new FunctionCommand { CommandId = "CREATE", FunctionId = function.Id });
                            context.FunctionCommands.Add(new FunctionCommand { CommandId = "UPDATE", FunctionId = function.Id });
                            context.FunctionCommands.Add(new FunctionCommand { CommandId = "DELETE", FunctionId = function.Id });
                            context.FunctionCommands.Add(new FunctionCommand { CommandId = "READ", FunctionId = function.Id });
                        }
                        context.SaveChanges();
                    }

                    if (!context.Permissions.Any())
                    {
                        var adminRole = roleManager.FindByNameAsync(adminRoleName).Result;
                        foreach (var function in functions)
                        {
                            context.Permissions.Add(new Permission { FunctionId = function.Id, CommandId = "CREATE", RoleId = adminRole.Id });
                            context.Permissions.Add(new Permission { FunctionId = function.Id, CommandId = "UPDATE", RoleId = adminRole.Id });
                            context.Permissions.Add(new Permission { FunctionId = function.Id, CommandId = "DELETE", RoleId = adminRole.Id });
                            context.Permissions.Add(new Permission { FunctionId = function.Id, CommandId = "READ", RoleId = adminRole.Id });
                        }
                        context.SaveChanges();
                    }
                    #endregion Thêm các lệnh cho chức năng & phân quyền

                    #region Danh mục tình trạng
                    if (!context.StatusCategories.Any())
                    {
                        context.StatusCategories.AddRange(new List<StatusCategory>
                        {
                            new StatusCategory { Id = "STATUS", Name = "Tình trạng chung" },

                            new StatusCategory { Id = "STATUS_EMPTY", Name = "Trống", ParentId = "STATUS", Color = "orange" },
                            new StatusCategory { Id = "STATUS_RECEIVED", Name = "Đã tiếp nhận", ParentId = "STATUS", Color = "blue" },
                            new StatusCategory { Id = "STATUS_COMPLETED", Name = "Hoàn tất", ParentId = "STATUS", Color = "green" },
                            new StatusCategory { Id = "STATUS_DELETED", Name = "Đã huỷ", ParentId = "STATUS", Color = "red" },

                            new StatusCategory { Id = "STATUS_PAYMENT", Name = "Tình trạng thanh toán" },

                            new StatusCategory { Id = "STATUS_PAYMENT_PAID", Name = "Đã thanh toán", ParentId = "STATUS_PAYMENT", Color = "green" },
                            new StatusCategory { Id = "STATUS_PAYMENT_UNPAID", Name = "Chưa thanh toán", ParentId = "STATUS_PAYMENT", Color = "red" },
                        });
                        context.SaveChanges();
                    }
                    #endregion Danh mục tình trạng

                    #region Phương thức thanh toán
                    if (!context.PaymentMethods.Any())
                    {
                        context.PaymentMethods.AddRange(new List<PaymentMethod>
                        {
                            new PaymentMethod { Id = "METHOD_DIRECT", Name = "Thanh toán trực tiếp" },
                            new PaymentMethod { Id = "METHOD_MOMO", Name = "Thanh toán MoMo" },
                        });
                        context.SaveChanges();
                    }
                    #endregion Phương thức thanh toán

                    #region Loại dịch vụ
                    if (!context.MedicalServiceTypes.Any())
                    {
                        context.MedicalServiceTypes.AddRange(new List<MedicalServiceType>
                        {
                            new MedicalServiceType { Id = "MED_SER_PHOBIEN", Name = "Dịch vụ phổ biến", Icon = "fa-briefcase-medical", SortOrder = 1 },
                            new MedicalServiceType { Id = "MED_SER_KHAM_XN", Name = "Khám & Xét nghiệm", Icon = "fa-vial", SortOrder = 2 },
                            new MedicalServiceType { Id = "MED_SER_XN_GAN", Name = "Xét nghiệm Gen", Icon = "fa-user-md", SortOrder = 3 },
                            new MedicalServiceType { Id = "MED_SER_SK_SD", Name = "Sức Khỏe & Sắc Đẹp", Icon = "fa-dna", SortOrder = 4 },
                            new MedicalServiceType { Id = "MED_SER_XN_TN", Name = "Xét nghiệm tại nhà", Icon = "fa-hand-holding-medical", SortOrder = 5 },
                        });
                        context.SaveChanges();
                    }
                    #endregion Loại dịch vụ
                }
            }
        }
    }
}

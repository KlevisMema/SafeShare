﻿using MudBlazor;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Internal;
using SafeShare.ClientDTO.Authentication;
using SafeShare.ClientServices.Interfaces;
using SafeShare.ClientDTO.AccountManagment;
using Microsoft.AspNetCore.Components.Forms;

namespace SafeShare.Client.Shared.Forms.Account;

public partial class EditProfileForm
{
    [Inject] private ISnackbar _snackbar { get; set; } = null!;

    [Inject]
    private IClientService_UserManagment _userManagmentService { get; set; } = null!;

    private ClientDto_UserInfo? UserInfo { get; set; }

    private ClientDto_UpdateUser UpdateUser { get; set; } = new();

    private EditForm? editProfileForm;

    private bool _processing = false;
    private bool _processingUploadProfilePic = false;
    private IBrowserFile? file;
    private const string EditUserDataModeInvalidGender = "Register - Please select a valid gender";
    private string BtnMessage { get; set; }

    private string? CurrentUserName;

    protected override async Task OnInitializedAsync()
    {
        UserInfo ??= new();

        var getUserInfo = await _userManagmentService.GetUser();

        if (getUserInfo.Succsess)
        {
            UserInfo = getUserInfo.Value;

            UpdateUser.Birthday = UserInfo.Birthday;
            UpdateUser.Enable2FA = UserInfo.RequireOTPDuringLogin;
            UpdateUser.FullName = UserInfo.FullName;
            UpdateUser.Gender = UserInfo.Gender;
            UpdateUser.PhoneNumber = UserInfo.PhoneNumber;
            UpdateUser.UserName = UserInfo.UserName;

            CurrentUserName = new string(UpdateUser.UserName);

            if (UserInfo.ProfilePicture is null || UserInfo.ProfilePicture.Length == 0)
                BtnMessage = "Add a profile picture";
            else
                BtnMessage = "Edit profile picture";
        }
    }

    private async Task
    ValidateEditUserDataForm()
    {
        var validationPassed = editProfileForm!.EditContext!.Validate();

        if (validationPassed)
        {
            if (UpdateUser.Gender == ClientDTO.Enums.Gender.SelectGender)
            {
                _snackbar.Add(EditUserDataModeInvalidGender, Severity.Warning, config => { config.CloseAfterNavigation = true; });
                return;
            }

            await SubmitUserDataUpdateForm();
        }
        else
            ShowValidationsMessages(editProfileForm.EditContext.GetValidationMessages());
    }

    private async Task
    SubmitUserDataUpdateForm()
    {
        _processing = true;
        await Task.Delay(1000);

        var updateDataResult = await _userManagmentService.UpdateUser(UpdateUser);

        if (!updateDataResult.Succsess)
        {
            UpdateUser.UserName = CurrentUserName?? UpdateUser.UserName;
            
            _snackbar.Add(updateDataResult.Message, Severity.Warning, config => { config.CloseAfterNavigation = true; });

            if (updateDataResult.Errors is not null)
                ShowValidationsMessages(updateDataResult.Errors!);
        }
        else
        {
            UserInfo.Age = DateTime.UtcNow.Year - UpdateUser.Birthday.Year;
            UserInfo.Gender = UpdateUser.Gender;
            _snackbar.Add(updateDataResult.Message, Severity.Success, config => { config.CloseAfterNavigation = true; });
        }

        _processing = false;
    }

    private void
    UploadFile
    (
        IBrowserFile file
    )
    {
        if (!IsImageValidFile(file.ContentType))
        {
            IEnumerable<string> errors =
            [
                $"Invalid .{file.ContentType.Split("/")[1]} file format",
            ];

            ShowValidationsMessages(errors);
            return;
        }
        this.file = file;
    }

    private static bool
    IsImageValidFile
    (
        string contentType
    )
    {
        var allowedTypes = new[] { "image/jpg", "image/jpeg", "image/png" };
        return allowedTypes.Contains(contentType);
    }

    private void
    ClearSelection()
    {
        this.file = null;
    }

    private async void
    UploadProfilePicture()
    {
        if (file is not null)
        {
            _processingUploadProfilePic = true;
            await Task.Delay(1000);

            using var memoryStream = new MemoryStream();
            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);

            var resultUploadProfilePic = await _userManagmentService.UploadProfilePicture(file.Name, fileContent);

            if (!resultUploadProfilePic.Succsess && resultUploadProfilePic.Errors is not null)
                ShowValidationsMessages(resultUploadProfilePic.Errors);
            else
            {
                _snackbar.Add(resultUploadProfilePic.Message, Severity.Success, config => { config.CloseAfterNavigation = true; });
                UserInfo.ProfilePicture = resultUploadProfilePic.Value;
            }

            _processingUploadProfilePic = false;
            ClearSelection();
            await InvokeAsync(StateHasChanged);

            return;
        }

        IEnumerable<string> errors =
        [
            $"Please select a file!",
        ];

        ShowValidationsMessages(errors);

    }

    private void
    ShowValidationsMessages
    (
        IEnumerable<string> validationMessages
    )
    {
        foreach (var validationMessage in validationMessages)
        {
            _snackbar.Add(validationMessage, Severity.Warning, config => { config.CloseAfterNavigation = true; config.VisibleStateDuration = 3000; });
        }
    }
}
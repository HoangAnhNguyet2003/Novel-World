﻿using System.ComponentModel.DataAnnotations;

namespace NovelWorld.Dtos.Auth.AuthDtos
{
    public class EmailValidDto
    {
        private string _email;

        [Required]
        [StringLength(30, ErrorMessage = "Email must be between 3 and 30 characters long.", MinimumLength = 3)]
        public string Email
        {
            get => _email;
            set => _email = value?.Trim();
        }
    }
}

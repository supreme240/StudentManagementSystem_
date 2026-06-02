namespace Student_management_system.Models {
    public class ForgotPasswordViewModel {
        public string Email { get; set; }
        public string Number { get; set; }
        public string NewPassword { get; set; }
        public bool ShowReset { get; set; }
        public int UserId { get; set; }
        public string ErrorMessage { get; set; }
    }
}

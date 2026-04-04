using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DoctorBooking.API.Infrastructure.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ══════════════════════════════════════════════════════════
    // 1. Welcome email — sent when account is first created
    // ══════════════════════════════════════════════════════════
    public async Task SendWelcomeEmailAsync(string toEmail, string fullName, string role)
    {
        var roleEmoji = role switch
        {
            "Doctor" => "👨‍⚕️",
            "Admin" => "🛡️",
            _ => "🏥"
        };

        var roleMessage = role switch
        {
            "Doctor" => "You can now manage your appointments, set your availability, and chat with patients.",
            "Admin" => "You have full access to manage users, doctors, and appointments.",
            _ => "You can now search for doctors, book appointments, and chat with your healthcare providers."
        };

        var subject = $"Welcome to MediBook, {fullName}! 🎉";

        var html = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family:'Segoe UI',Arial,sans-serif;background:#f8f9fc;margin:0;padding:0">
              <div style="max-width:600px;margin:0 auto;padding:40px 20px">

                <!-- Header -->
                <div style="background:linear-gradient(135deg,#1e4d8c,#0f9a8a);border-radius:16px 16px 0 0;padding:40px;text-align:center">
                  <h1 style="color:#fff;margin:0;font-size:2rem">💊 MediBook</h1>
                  <p style="color:rgba(255,255,255,.85);margin:8px 0 0">Your Health, Our Priority</p>
                </div>

                <!-- Body -->
                <div style="background:#fff;border-radius:0 0 16px 16px;padding:40px;box-shadow:0 4px 20px rgba(0,0,0,.08)">
                  <h2 style="color:#1a1a2e;margin:0 0 16px">Welcome, {fullName}! {roleEmoji}</h2>
                  <p style="color:#475569;line-height:1.7;margin:0 0 24px">
                    Your MediBook account has been created successfully as a <strong>{role}</strong>.
                  </p>
                  <div style="background:#f0f4ff;border-radius:12px;padding:20px;margin:0 0 24px;border-left:4px solid #1e4d8c">
                    <p style="color:#1e4d8c;margin:0;font-weight:600">{roleMessage}</p>
                  </div>

                  <!-- Features -->
                  <h3 style="color:#1a1a2e;margin:24px 0 16px">What you can do:</h3>
                  <table style="width:100%;border-collapse:separate;border-spacing:0 8px">
                    <tr><td style="padding:12px;background:#f8f9fc;border-radius:8px;color:#475569">📅 Book and manage appointments</td></tr>
                    <tr><td style="padding:12px;background:#f8f9fc;border-radius:8px;color:#475569">💬 Chat with doctors in real-time</td></tr>
                    <tr><td style="padding:12px;background:#f8f9fc;border-radius:8px;color:#475569">🔔 Get email and in-app reminders</td></tr>
                    <tr><td style="padding:12px;background:#f8f9fc;border-radius:8px;color:#475569">⭐ Rate and review your appointments</td></tr>
                  </table>

                  <!-- CTA -->
                  <div style="text-align:center;margin:32px 0">
                    <a href="http://localhost:4200/auth/login"
                       style="background:linear-gradient(135deg,#1e4d8c,#0f9a8a);color:#fff;padding:14px 36px;border-radius:10px;text-decoration:none;font-weight:600;font-size:1rem;display:inline-block">
                      Get Started →
                    </a>
                  </div>

                  <p style="color:#94a3b8;font-size:.85rem;text-align:center;margin:0">
                    If you didn't create this account, please ignore this email.<br>
                    © 2024 MediBook. All rights reserved.
                  </p>
                </div>

              </div>
            </body>
            </html>
            """;

        await SendAsync(toEmail, subject, html);
    }

    // ══════════════════════════════════════════════════════════
    // 2. Appointment booking confirmation — sent right after booking
    // ══════════════════════════════════════════════════════════
    public async Task SendAppointmentConfirmationAsync(
        string toEmail, string patientName, string doctorName,
        DateTime date, string timeSlot, decimal fee, string reason)
    {
        var subject = "✅ Appointment Confirmed — MediBook";

        var html = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family:'Segoe UI',Arial,sans-serif;background:#f8f9fc;margin:0;padding:0">
              <div style="max-width:600px;margin:0 auto;padding:40px 20px">

                <!-- Header -->
                <div style="background:linear-gradient(135deg,#198754,#0f9a8a);border-radius:16px 16px 0 0;padding:40px;text-align:center">
                  <div style="width:70px;height:70px;background:rgba(255,255,255,.2);border-radius:50%;display:flex;align-items:center;justify-content:center;margin:0 auto 16px">
                    <span style="font-size:2rem">✅</span>
                  </div>
                  <h1 style="color:#fff;margin:0;font-size:1.6rem">Appointment Confirmed!</h1>
                </div>

                <!-- Body -->
                <div style="background:#fff;border-radius:0 0 16px 16px;padding:40px;box-shadow:0 4px 20px rgba(0,0,0,.08)">
                  <p style="color:#475569;margin:0 0 24px">Hello <strong>{patientName}</strong>, your appointment has been confirmed.</p>

                  <!-- Appointment Details Card -->
                  <div style="background:#f0f4ff;border-radius:12px;padding:24px;margin:0 0 24px">
                    <h3 style="color:#1e4d8c;margin:0 0 16px">📋 Appointment Details</h3>
                    <table style="width:100%;border-collapse:collapse">
                      <tr><td style="padding:8px 0;color:#64748b;width:40%">Doctor</td><td style="padding:8px 0;color:#1a1a2e;font-weight:600">Dr. {doctorName}</td></tr>
                      <tr><td style="padding:8px 0;color:#64748b;border-top:1px solid #e2e8f0">Date</td><td style="padding:8px 0;color:#1a1a2e;font-weight:600;border-top:1px solid #e2e8f0">{date:dddd, MMMM dd, yyyy}</td></tr>
                      <tr><td style="padding:8px 0;color:#64748b;border-top:1px solid #e2e8f0">Time</td><td style="padding:8px 0;color:#1a1a2e;font-weight:600;border-top:1px solid #e2e8f0">{timeSlot}</td></tr>
                      <tr><td style="padding:8px 0;color:#64748b;border-top:1px solid #e2e8f0">Reason</td><td style="padding:8px 0;color:#1a1a2e;font-weight:600;border-top:1px solid #e2e8f0">{reason}</td></tr>
                      <tr><td style="padding:8px 0;color:#64748b;border-top:1px solid #e2e8f0">Consultation Fee</td><td style="padding:8px 0;color:#0f9a8a;font-weight:700;font-size:1.1rem;border-top:1px solid #e2e8f0">₹{fee}</td></tr>
                    </table>
                  </div>

                  <!-- Reminder tip -->
                  <div style="background:#fff3cd;border-radius:10px;padding:16px;margin:0 0 24px;border-left:4px solid #ffc107">
                    <p style="color:#92400e;margin:0;font-size:.9rem">
                      ⏰ <strong>Tip:</strong> Please arrive 10 minutes early. You will receive a reminder email the day before your appointment.
                    </p>
                  </div>

                  <!-- CTA -->
                  <div style="text-align:center;margin:24px 0">
                    <a href="http://localhost:4200/patient/my-appointments"
                       style="background:linear-gradient(135deg,#1e4d8c,#0f9a8a);color:#fff;padding:14px 36px;border-radius:10px;text-decoration:none;font-weight:600;display:inline-block">
                      View My Appointments
                    </a>
                  </div>

                  <p style="color:#94a3b8;font-size:.85rem;text-align:center;margin:0">
                    © 2024 MediBook. If you did not book this appointment, please contact us immediately.
                  </p>
                </div>

              </div>
            </body>
            </html>
            """;

        await SendAsync(toEmail, subject, html);
    }

    // ══════════════════════════════════════════════════════════
    // 3. Appointment reminder — sent the day before
    // ══════════════════════════════════════════════════════════
    public async Task SendAppointmentReminderAsync(
        string toEmail, string patientName, string doctorName,
        DateTime date, string timeSlot)
    {
        var subject = "⏰ Appointment Reminder — Tomorrow — MediBook";

        var html = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family:'Segoe UI',Arial,sans-serif;background:#f8f9fc;margin:0;padding:0">
              <div style="max-width:600px;margin:0 auto;padding:40px 20px">

                <!-- Header -->
                <div style="background:linear-gradient(135deg,#f59e0b,#f97316);border-radius:16px 16px 0 0;padding:40px;text-align:center">
                  <span style="font-size:3rem">⏰</span>
                  <h1 style="color:#fff;margin:12px 0 0;font-size:1.6rem">Appointment Reminder</h1>
                  <p style="color:rgba(255,255,255,.9);margin:8px 0 0">You have an appointment <strong>tomorrow</strong></p>
                </div>

                <!-- Body -->
                <div style="background:#fff;border-radius:0 0 16px 16px;padding:40px;box-shadow:0 4px 20px rgba(0,0,0,.08)">
                  <p style="color:#475569;margin:0 0 24px">Hello <strong>{patientName}</strong>, this is a reminder for your upcoming appointment.</p>

                  <!-- Details -->
                  <div style="background:#fff3cd;border-radius:12px;padding:24px;margin:0 0 24px;border:2px solid #ffc107">
                    <h3 style="color:#92400e;margin:0 0 16px">📅 Tomorrow's Appointment</h3>
                    <table style="width:100%;border-collapse:collapse">
                      <tr><td style="padding:8px 0;color:#64748b;width:40%">Doctor</td><td style="padding:8px 0;color:#1a1a2e;font-weight:600">Dr. {doctorName}</td></tr>
                      <tr><td style="padding:8px 0;color:#64748b;border-top:1px solid #fde68a">Date</td><td style="padding:8px 0;color:#1a1a2e;font-weight:600;border-top:1px solid #fde68a">{date:dddd, MMMM dd, yyyy}</td></tr>
                      <tr><td style="padding:8px 0;color:#64748b;border-top:1px solid #fde68a">Time</td><td style="padding:8px 0;color:#1a1a2e;font-weight:600;border-top:1px solid #fde68a">{timeSlot}</td></tr>
                    </table>
                  </div>

                  <div style="background:#f0fdf4;border-radius:10px;padding:16px;margin:0 0 24px">
                    <p style="color:#166534;margin:0;font-size:.9rem">
                      ✅ Please arrive 10 minutes early and bring any relevant medical documents.
                    </p>
                  </div>

                  <div style="text-align:center;margin:24px 0">
                    <a href="http://localhost:4200/patient/my-appointments"
                       style="background:linear-gradient(135deg,#f59e0b,#f97316);color:#fff;padding:14px 36px;border-radius:10px;text-decoration:none;font-weight:600;display:inline-block">
                      View Appointment
                    </a>
                  </div>

                  <p style="color:#94a3b8;font-size:.85rem;text-align:center;margin:0">© 2024 MediBook</p>
                </div>
              </div>
            </body>
            </html>
            """;

        await SendAsync(toEmail, subject, html);
    }

    // ══════════════════════════════════════════════════════════
    // 4. Password reset OTP — sent when user requests reset
    // ══════════════════════════════════════════════════════════
    public async Task SendPasswordResetOtpAsync(string toEmail, string fullName, string otp)
    {
        var subject = " Password Reset OTP — MediBook";

        var html = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family:'Segoe UI',Arial,sans-serif;background:#f8f9fc;margin:0;padding:0">
              <div style="max-width:600px;margin:0 auto;padding:40px 20px">

                <div style="background:linear-gradient(135deg,#1e4d8c,#0f9a8a);border-radius:16px 16px 0 0;padding:40px;text-align:center">
                  <span style="font-size:3rem">🔐</span>
                  <h1 style="color:#fff;margin:12px 0 0;font-size:1.6rem">Password Reset Request</h1>
                </div>

                <div style="background:#fff;border-radius:0 0 16px 16px;padding:40px;box-shadow:0 4px 20px rgba(0,0,0,.08)">
                  <p style="color:#475569;margin:0 0 24px">Hello <strong>{fullName}</strong>, use the OTP below to reset your password.</p>

                  <!-- OTP Box -->
                  <div style="text-align:center;margin:32px 0">
                    <div style="background:#f0f4ff;border-radius:16px;padding:32px;display:inline-block;min-width:200px">
                      <p style="color:#64748b;margin:0 0 8px;font-size:.85rem;text-transform:uppercase;letter-spacing:2px">Your OTP</p>
                      <div style="font-size:2.5rem;font-weight:800;color:#1e4d8c;letter-spacing:12px;font-family:monospace">
                        {otp}
                      </div>
                      <p style="color:#f59e0b;margin:8px 0 0;font-size:.85rem;font-weight:600">
                        ⏱ Valid for 10 minutes only
                      </p>
                    </div>
                  </div>

                  <div style="background:#fee2e2;border-radius:10px;padding:16px;margin:0 0 24px">
                    <p style="color:#991b1b;margin:0;font-size:.9rem">
                      🚨 <strong>Never share this OTP</strong> with anyone. MediBook will never ask for your OTP.
                    </p>
                  </div>

                  <p style="color:#94a3b8;font-size:.85rem;text-align:center;margin:0">
                    If you didn't request a password reset, ignore this email.<br>
                    © 2024 MediBook
                  </p>
                </div>
              </div>
            </body>
            </html>
            """;

        await SendAsync(toEmail, subject, html);
    }

    // ══════════════════════════════════════════════════════════
    // Private: Core send method using MailKit + Gmail SMTP
    // ══════════════════════════════════════════════════════════
    private async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var es = _config.GetSection("EmailSettings");
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(es["FromName"]!, es["FromEmail"]!));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();

            // Gmail: port 587 = STARTTLS (not SSL)
            await client.ConnectAsync(
                es["Host"]!,
                int.Parse(es["Port"]!),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                es["Username"]!,
                es["Password"]!);

            await client.SendAsync(msg);
            await client.DisconnectAsync(true);

            _logger.LogInformation("✅ Email sent → {Email} | {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            // Log the FULL error so you can debug
            _logger.LogError(ex, "❌ Email FAILED → {Email} | Error: {Message}", toEmail, ex.Message);
        }
    }
}
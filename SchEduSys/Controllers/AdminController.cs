using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using SchEduSys.Models;

namespace SchEduSys.Controllers
{

    public class AdminController : Controller
    {
        public SchEduSysEntities schEduSysEntities = new SchEduSysEntities();

        //获取登录页面
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //添加用户
        public bool AddAdmin(String adminName, String adminPassword, String adminGender, String adminTelephone, String adminEmail, String adminRealName, String adminIdCard)
        {
            admin administrator = schEduSysEntities.admin.FirstOrDefault(ad => ad.adminName == adminName);
            if (administrator != null)
            {
                return false;
            }
            admin newadmin = new admin()
            {
                adminName = adminName,
                adminPassword = adminPassword,
                adminGender = adminGender,
                adminTelephone = adminTelephone,
                adminEmail = adminEmail,
                adminRealName = adminRealName,
                adminIdCard = adminIdCard
            }; try
            {
                schEduSysEntities.admin.Add(newadmin);
                schEduSysEntities.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        //提交登录信息
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(String adminName, String adminPassword, String adminCode)
        {
            admin administrator = schEduSysEntities.admin.FirstOrDefault(ad => ad.adminName == adminName);
            if (String.IsNullOrEmpty(adminCode))
            {
                return Content("验证码不能为空");
            }
            if (String.IsNullOrEmpty(adminName))
            {
                return Content("用户名不能为空");
            }
            if (String.IsNullOrEmpty(adminPassword))
            {
                return Content("密码不能为空");
            }
            else
            {
                String sessionSecurityCode = Session["SecurityCode"] as String;
                if (administrator == null)
                {
                    return Content("用户不存在");
                }
                if (administrator.adminPassword != adminPassword)
                {
                    return Content("密码错误");
                }
                if (adminCode != sessionSecurityCode)
                {
                    return Content("验证码错误");
                }
                //将登录成功的用户加入session。
                Session["loginInAdmin"] = administrator;
                return View();
            }
        }

        //获得所有用户
        public void GetAllAdimin()
        {
            List<admin> admins = schEduSysEntities.admin.ToList();
            ViewBag.admins = admins;
        }

        //找回密码验证
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool ResetPasswordCheck(String adminTelephone,String adminIdCard)
        {
            admin administrator = schEduSysEntities.admin.FirstOrDefault(ad => ad.adminTelephone == adminTelephone);
            if (administrator == null)
            {
                ViewBag.ResetCheckErrorLog = "用户名不存在";
                return false;
            }
            if (adminIdCard.Equals(administrator.adminIdCard))
            {
                ViewBag.resetAdminId = administrator.adminId;
            }
            return true;
        }

        //重置密码
        [HttpPost]
        public bool ResetPassword(String newPassword)
        {
            int resetAdminId = ViewBag.resetAdminId;//获取正准备重置的用户
            admin administrator = schEduSysEntities.admin.Find(resetAdminId);
            if(administrator==null)
            {
                ViewBag.ResetErrorLog = "要重置的用户不存在！";
                return false;
            }
            administrator.adminPassword = newPassword;
            schEduSysEntities.SaveChanges();
            return true;
        }

        //验证码
        public ActionResult SecurityCode()
        {
            String oldcode = Session["SecurityCode"] as String;
            String code = CreateRandomCode(5);
            Session["SecurityCode"] = code;
            return File(CreateValidateGraphic(code), "image/Jpeg");
        }

        private byte[] CreateImage(String checkCode)
        {
            int iwidth = checkCode.Length * 12;
            Bitmap image = new Bitmap(iwidth, 20);
            Graphics g = Graphics.FromImage(image);
            Font f = new Font("Arial", 10, FontStyle.Bold);
            Brush b = new SolidBrush(Color.White);
            g.Clear(Color.Blue);
            g.DrawString(checkCode, f, b, 3, 3);
            Pen blackPen = new Pen(Color.Black, 0);
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                int x1 = rand.Next(image.Width);
                int x2 = rand.Next(image.Width);
                int y1 = rand.Next(image.Height);
                int y2 = rand.Next(image.Height);
                g.DrawLine(new Pen(Color.Silver, 1), x1, y1, x2, y2);
            }
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        //创建随机验证码
        private String CreateRandomCode(int codeCount)

        {
            String allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";
            String[] allCharArray = allChar.Split(',');
            String randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(35);
                if (temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        //创建验证码的图片
        public byte[] CreateValidateGraphic(String validateCode)
        {
            Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 16.0), 27);
            Graphics g = Graphics.FromImage(image);
            try
            {
                //生成随机生成器
                Random random = new Random();
                //清空图片背景色
                g.Clear(Color.White);
                //画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 13, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                 Color.Blue, Color.DarkRed, 1.2f, true);
                g.DrawString(validateCode, font, brush, 3, 2);
                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
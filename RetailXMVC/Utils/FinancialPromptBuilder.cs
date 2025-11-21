using System;
using System.Collections.Generic;
using System.Linq;

namespace RetailXMVC.Utils
{
    public static class FinancialPromptBuilder
    {
        // ✅ METHOD CŨ - Phân tích tự động cho Chart
        public static string BuildAnalysisPrompt(
            int year,
            List<MonthlyFinancialData> monthlyData,
            decimal totalRevenue,
            decimal totalCost,
            decimal totalSalary,
            decimal totalProfit)
        {
            var profitMargin = totalRevenue > 0 ? (totalProfit / totalRevenue * 100) : 0;
            var costRatio = totalRevenue > 0 ? (totalCost / totalRevenue * 100) : 0;
            var salaryRatio = totalRevenue > 0 ? (totalSalary / totalRevenue * 100) : 0;

            var activeMonths = monthlyData.Where(m => m.Revenue > 0 || m.Cost > 0 || m.Salary > 0).ToList();
            var inactiveMonthsCount = 12 - activeMonths.Count;

            var monthlyDetails = string.Join("\n", activeMonths.Select(m =>
                $"  • Tháng {m.Month}: Doanh thu {m.Revenue:N0} VNĐ, Chi phí nhập {m.Cost:N0} VNĐ, Lương {m.Salary:N0} VNĐ, Lợi nhuận {m.Profit:N0} VNĐ"
            ));

            var bestMonth = activeMonths.OrderByDescending(m => m.Profit).FirstOrDefault();
            var worstMonth = activeMonths.OrderBy(m => m.Profit).FirstOrDefault();

            string trendAnalysis = "";
            if (activeMonths.Count >= 3)
            {
                var firstThreeAvg = activeMonths.Take(3).Average(m => m.Revenue);
                var lastThreeAvg = activeMonths.TakeLast(3).Average(m => m.Revenue);
                var trendPercent = firstThreeAvg > 0 ? ((lastThreeAvg - firstThreeAvg) / firstThreeAvg * 100) : 0;

                if (trendPercent > 10)
                    trendAnalysis = $"📈 Xu hướng TĂNG {trendPercent:N1}% (3 tháng đầu vs 3 tháng cuối)";
                else if (trendPercent < -10)
                    trendAnalysis = $"📉 Xu hướng GIẢM {Math.Abs(trendPercent):N1}% (3 tháng đầu vs 3 tháng cuối)";
                else
                    trendAnalysis = "📊 Xu hướng ỔN ĐỊNH (dao động < 10%)";
            }

            var prompt = $@"
Bạn là chuyên gia phân tích tài chính cho doanh nghiệp bán lẻ RetailX tại Việt Nam.
Hãy phân tích dữ liệu kinh doanh năm {year} theo phong cách chuyên nghiệp, dễ hiểu.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📊 TỔNG QUAN NĂM {year}
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

💰 Tổng doanh thu:        {totalRevenue:N0} VNĐ
📦 Chi phí nhập hàng:     {totalCost:N0} VNĐ ({costRatio:N1}% doanh thu)
👥 Chi phí lương:         {totalSalary:N0} VNĐ ({salaryRatio:N1}% doanh thu)
💵 Lợi nhuận ròng:        {totalProfit:N0} VNĐ
📈 Biên lợi nhuận:        {profitMargin:N1}%
📅 Tháng hoạt động:       {activeMonths.Count}/12 tháng

{trendAnalysis}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📈 CHI TIẾT TỪNG THÁNG
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

{monthlyDetails}

{(bestMonth != null ? $"\n🏆 Tháng tốt nhất: Tháng {bestMonth.Month} (Lợi nhuận {bestMonth.Profit:N0} VNĐ)" : "")}
{(worstMonth != null ? $"⚠️  Tháng thấp nhất: Tháng {worstMonth.Month} (Lợi nhuận {worstMonth.Profit:N0} VNĐ)" : "")}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

YÊU CẦU PHÂN TÍCH (NGẮN GỌN, CỤ THỂ):

1. **📈 Xu Hướng Kinh Doanh** (2-3 câu):
   - Nhận xét về tăng/giảm doanh thu qua các tháng
   - Có mùa vụ hoặc chu kỳ đặc biệt không?
   {(inactiveMonthsCount > 0 ? $"- Tại sao có {inactiveMonthsCount} tháng không hoạt động?" : "")}

2. **💪 Điểm Mạnh** (2-3 điểm, MỖI ĐIỂM 1 DÒNG):
   - Những thành tựu nổi bật
   - Chỉ số tài chính tích cực

3. **⚠️ Cảnh Báo & Rủi Ro** (1-2 điểm CỤ THỂ):
   - Vấn đề cần giải quyết NGAY
   - Tháng/chi phí bất thường

4. **💡 Khuyến Nghị** (3-4 hành động CỤ THỂ):
   - Cách tăng doanh thu ngay lập tức
   - Tối ưu chi phí ở đâu
   - Chiến lược cho năm {year + 1}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
QUY TẮC ĐỊNH DẠNG:
✅ Dùng emoji (📈💰⚠️💡🎯📊🏆)
✅ Mỗi phần 2-4 dòng, NGẮN GỌN
✅ **In đậm** từ khóa quan trọng
✅ KHÔNG dùng markdown heading (##, ###)
✅ Tối đa 350 từ
✅ Tiếng Việt, chuyên nghiệp nhưng thân thiện
";

            return prompt;
        }

        // ✅✅✅ METHOD MỚI - Chatbot tương tác ✅✅✅
        public static string BuildChatPrompt(
            string userQuestion,
            int year,
            List<MonthlyFinancialData> monthlyData)
        {
            var activeMonths = monthlyData.Where(m => m.Revenue > 0 || m.Cost > 0 || m.Salary > 0).ToList();

            var totalRevenue = monthlyData.Sum(m => m.Revenue);
            var totalCost = monthlyData.Sum(m => m.Cost);
            var totalSalary = monthlyData.Sum(m => m.Salary);
            var totalProfit = monthlyData.Sum(m => m.Profit);

            var profitMargin = totalRevenue > 0 ? (totalProfit / totalRevenue * 100) : 0;

            var monthlyDetails = string.Join("\n", activeMonths.Select(m =>
                $"  • Tháng {m.Month}: DT {m.Revenue:N0} VNĐ | Chi phí {m.Cost:N0} VNĐ | Lương {m.Salary:N0} VNĐ | Lãi {m.Profit:N0} VNĐ"
            ));

            var bestMonth = activeMonths.OrderByDescending(m => m.Profit).FirstOrDefault();
            var worstMonth = activeMonths.OrderBy(m => m.Profit).FirstOrDefault();

            // Tính xu hướng
            string trendInfo = "";
            if (activeMonths.Count >= 3)
            {
                var recentThree = activeMonths.TakeLast(3).Average(m => m.Revenue);
                var oldThree = activeMonths.Take(3).Average(m => m.Revenue);
                var trendPercent = oldThree > 0 ? ((recentThree - oldThree) / oldThree * 100) : 0;

                if (trendPercent > 5)
                    trendInfo = $"📈 Xu hướng: TĂNG {trendPercent:N1}% (3 tháng gần nhất vs 3 tháng đầu)";
                else if (trendPercent < -5)
                    trendInfo = $"📉 Xu hướng: GIẢM {Math.Abs(trendPercent):N1}%";
                else
                    trendInfo = "📊 Xu hướng: Ổn định";
            }

            var prompt = $@"
Bạn là **AI Financial Assistant** của doanh nghiệp bán lẻ RetailX tại Việt Nam.
Nhiệm vụ: Trả lời câu hỏi của người dùng về tài chính một cách CHÍNH XÁC, NGẮN GỌN, DỄ HIỂU.

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📊 DỮ LIỆU NĂM {year} (RetailX)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

**TỔNG QUAN:**
💰 Tổng doanh thu:    {totalRevenue:N0} VNĐ
📦 Chi phí nhập:      {totalCost:N0} VNĐ
👥 Chi phí lương:     {totalSalary:N0} VNĐ
💵 Lợi nhuận:         {totalProfit:N0} VNĐ
📈 Biên lợi nhuận:    {profitMargin:N1}%
📅 Tháng hoạt động:   {activeMonths.Count}/12 tháng

{trendInfo}

**CHI TIẾT TỪNG THÁNG:**
{monthlyDetails}

{(bestMonth != null ? $"\n🏆 Tháng tốt nhất: T{bestMonth.Month} (Lãi {bestMonth.Profit:N0} VNĐ)" : "")}
{(worstMonth != null ? $"🔴 Tháng thấp nhất: T{worstMonth.Month} (Lãi {worstMonth.Profit:N0} VNĐ)" : "")}

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
❓ CÂU HỎI CỦA NGƯỜI DÙNG:
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

""{userQuestion}""

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
📋 QUY TẮC TRẢ LỜI (BẮT BUỘC):
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

✅ **NGẮN GỌN**: Tối đa 3-5 câu (100-150 từ)
✅ **CỤ THỂ**: Trích dẫn SỐ LIỆU từ dữ liệu trên
✅ **DỄ HIỂU**: Giải thích đơn giản, không dùng thuật ngữ phức tạp
✅ **EMOJI**: Dùng 📈📉💰💡⚠️🏆 để dễ đọc
✅ **SO SÁNH**: Nếu hỏi so sánh → Nêu rõ số liệu và % chênh lệch
✅ **KHUYẾN NGHỊ**: Nếu phù hợp → Đưa 1-2 gợi ý ngắn gọn

**XỬ LÝ CÂU HỎI ĐẶC BIỆT:**

🔹 Nếu hỏi về tháng KHÔNG CÓ DỮ LIỆU:
   → ""Tháng X/{year} chưa có hoạt động kinh doanh (doanh thu = 0 VNĐ).""

🔹 Nếu hỏi NGOÀI phạm vi tài chính RetailX:
   → ""Tôi chỉ hỗ trợ phân tích tài chính RetailX năm {year}. Bạn có thể hỏi về doanh thu, chi phí, lợi nhuận, xu hướng, v.v.""

🔹 Nếu hỏi dự đoán tương lai:
   → ""Dựa trên xu hướng hiện tại... [phân tích ngắn gọn dựa trên data]""

🔹 Nếu hỏi khuyến nghị:
   → Đưa 2-3 hành động CỤ THỂ, THỰC TẾ

**VÍ DỤ TRẢ LỜI TốT:**

Câu hỏi: ""Doanh thu tháng 11 bao nhiêu?""
Trả lời: 
""📊 **Doanh thu tháng 11/{year}: 25,365,000 VNĐ**

So với tháng 10 (50,000,000 VNĐ) → Giảm **24,635,000 VNĐ (-49.3%)** 📉

💡 Cần phân tích nguyên nhân giảm mạnh để điều chỉnh chiến lược kinh doanh.""

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

QUAN TRỌNG: 
- TRẢ LỜI NGAY, KHÔNG lặp lại câu hỏi
- DÙNG SỐ LIỆU từ data trên
- NGẮN GỌN, dễ hiểu như nói chuyện với đồng nghiệp
";

            return prompt;
        }
    }

    public class MonthlyFinancialData
    {
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public decimal Cost { get; set; }
        public decimal Salary { get; set; }
        public decimal Profit { get; set; }
    }
}
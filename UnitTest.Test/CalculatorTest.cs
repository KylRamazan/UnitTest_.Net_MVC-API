using Moq;
using UnitTest.App;

namespace UnitTest.Test
{
  public class CalculatorTest
  {
    private Calculator _calculator;
    public Mock<ICalculatorService> myMock;//3. parti uygulamalara istek atıp sonucun gelmesi işlemini test etmemek için Mock yapısı kullanılır. Verilen Interface veya sınıfı kopyalar.

    public CalculatorTest()
    {
      myMock = new Mock<ICalculatorService>();
      _calculator = new Calculator(myMock.Object);
      //CalculatorService cal = new CalculatorService();
      //_calculator = new Calculator(cal);
    }


    [Fact]//Method parametre almadığında kullanılır
    public void AddTest()
    {
      //Test 3 asamadan olusmaktadir.
      
      //Arrange (Degiskenler ve nesneler tanimlanir.)
      int a = 5, b = 20;

      //Act (Gerekli methodlarin test islemler gerceklestirilir.)
      int sonuc = _calculator.Add(a, b);

      //Assert (Sonucun dogrulugu kontrol edilir.)
      Assert.Equal(25, sonuc);


      #region Assert Methodları
      //* Tüm methodlar 2 parametre alır. Soldaki alınacak sonuç, sağdaki ise test sonucunu alan parametredir.*//

      Assert.Contains("Fatih", "Fatih Çakıroğlu");//Verilen ifade içerisinde geçip geçmediğini kontrol eder.
      Assert.DoesNotContain("Emre", "Fatih Çakıroğlu");//Verilen ifade içerisinde yok ise test başarılı, var ise test başarısız olur.
      List<string> names = new List<string>
      {
        "Fatih",
        "Emre",
        "Hasan"
      };
      Assert.Contains(names, x => x == "Fatih");//Liste içerisinde eleman araması yapılır, aranan deger var ise test başarılı, yok ise test başarısız olur.


      Assert.True(5 > 2);//Sart dogru ise test başarılı, yanlış ise test başarısız olur.
      Assert.False(5 < 2);//Sart dogru ise test başarısız, yanlış ise test başarılı olur.


      string regex = "^dog";//dog ile başlamalı
      Assert.Matches(regex, "dog Fatih");//Verilen regex kontrolünü sağlar, uyuyorsa test başarılı, uymuyorsa test başarısız olur.
      Assert.DoesNotMatch(regex, "dog Fatih");//Verilen regex kontrolünü sağlar, uyuyorsa test başarısız, uymuyorsa test başarılı olur.


      Assert.StartsWith("Bir", "Bir masal");//Verilen ifade ile başlıyorsa test başarılı, başlamıyor ise test başarısız olur.
      Assert.EndsWith("masal", "Bir masal");//Verilen ifade ile bitiyorsa test başarılı, bitmiyor ise test başarısız olur.


      Assert.Empty(new List<string>());//Dizin boş ise test başarılı, boş değilse test başarısız olur.
      Assert.NotEmpty(new List<string>() { "Fatih" });//Dizin boş ise test başarısız, boş değilse test başarılı olur.


      Assert.InRange(10, 2, 20);//Verilen değer 2 ile 20 arasında ise test başarılı, değil ise test başarısız olur.
      Assert.NotInRange(22, 2, 20);//Verilen değer 2 ile 20 arasında ise test başarısız, değil ise test başarılı olur.


      Assert.Single(new List<string>() { "Fatih" });//Verilen liste veya dizide bir eleman varsa test başarılı, birden fazla eleman varsa test başarısız olur.


      Assert.IsType<string>("Fatih");//Verilen ifade tipi uyuyorsa test başarılı, uymuyorsa test başarısız olur.
      Assert.IsNotType<int>("Fatih");//Verilen ifade tipi uyuyorsa test başarısız, uymuyorsa test başarılı olur.


      Assert.IsAssignableFrom<IEnumerable<string>>(new List<string>() { "Fatih" });// Verilen ifade miras-kalıtım alıyorsa test başarılı, almıyorsa test başarısız olur.
      Assert.IsAssignableFrom<object>("Fatih");


      string value = null;
      Assert.Null(value);//Verilen ifade null ise test başarılı, değil ise test başarısız olur.
      string value2 = "Fatih";
      Assert.NotNull(value2);//Verilen ifade null ise test başarısız, değil ise test başarılı olur.


      Assert.Equal(2, 2);//Verilen deger(soldaki), beklenen değer(sağdaki) ile aynı ise test başarılı, değil ise test başarısız olur.
      Assert.NotEqual(3, 2);//Verilen deger(soldaki), beklenen değer(sağdaki) ile aynı ise test başarısız, değil ise test başarılı olur.
      #endregion
    }


    [Theory]//Method parametre aldığında kullanılır
    [InlineData(2, 5, 7)]//Parametrelerin degerleri bu method ile sırasıyla belirlenir. Birden fazla senaryo için ise InlineData tekrar eklenir.
    [InlineData(10, -2, 8)]
    public void Add_SimpleValues_ReturnTotalValue(int a, int b, int expectedTotal)
    {
      int actualTotal = _calculator.Add(a, b);
      Assert.Equal(expectedTotal, actualTotal);
    }

    [Theory]
    [InlineData(0, 5, 0)]
    [InlineData(10, 0, 0)]
    public void Add_ZeroValues_ReturnZeroValue(int a, int b, int expectedTotal)
    {
      int actualTotal = _calculator.Add(a, b);

      Assert.Equal(expectedTotal, actualTotal);
    }

    [Theory]
    [InlineData(3, 5, 15)]
    public void Multip_SimpleValues_ReturnMultipValue(int a, int b, int expectedTotal)
    {
      myMock.Setup(x => x.Multip(a, b)).Returns(expectedTotal);//Kopyalanan Interface veya sınıf içindeki method seçilir ve dönüş durumu da verilir. Yani Multip methoduna hiç girmeden parametrelerde verilen değerleri alır ve döner.
      int actualTotal = _calculator.Multip(a, b);//Sahte interface üzerinden Multip methodu çalıştırılır ve içine girmez. Parametrelerdeki değerlei döner.

      Assert.Equal(expectedTotal, actualTotal);
      myMock.Verify(x => x.Multip(a, b), Times.Once);//Multip methodu 1 kez çalışırsa test başarılı olur, 1'den fazla kez çalışırsa test başarısız olur.
      //myMock.Verify(x => x.Multip(a, b), Times.AtLeast(2));//Multip methodu en az 2 kez çalışırsa test başarılı olur, 2'den az çalışırsa test başarısız olur.
      //myMock.Verify(x => x.Multip(a, b), Times.AtMost(3));//Multip methodu en fazla 3 kez çalışırsa test başarılı olur, 3'den fazla çalışırsa test başarısız olur.
    }

    [Theory]
    [InlineData(0, 5)]
    public void Multip_ZeroValue_ReturnException(int a, int b)
    {
      myMock.Setup(x => x.Multip(a, b)).Throws(new Exception("a'nın değeri 0 olamaz!"));//Kopyalanan Interface veya sınıf içindeki method hata dönüyorsa bunu simüle etmek için throws kullanılır.
      Exception exp = Assert.Throws<Exception>(() => _calculator.Multip(a, b));//Multip methodu exception fırlatacak ve bunu değişkene atayacak.
      Assert.Equal("a'nın değeri 0 olamaz!", exp.Message);//Gelen exception mesajı ile bizim beklediğimiz mesaj aynı ise test başarılı olacak, değil ise test başarısız olacak.

      // Not: Birden fazla Assert methodu kullanılabilir, hepsi true ise test başarılı olur, bir tanesi false ise test başarısız olur.
    }
  }
}

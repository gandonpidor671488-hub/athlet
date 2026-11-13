using SQLite;
namespace Athlet;

public class Product
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Category { get; set; } = "";
    public string Emoji { get; set; } = "";
    public string Weight { get; set; } = "";
    public string Description { get; set; } = "";
    public string Composition { get; set; } = "";
    public int Price { get; set; }
    [Ignore] public string PriceText => $"{Price:N0} ₽";
}
public class CartItem { [PrimaryKey] public int ProductId { get; set; } public int Qty { get; set; } }
public class Order
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Method { get; set; } = "";
    public string Address { get; set; } = "";
    public int Total { get; set; }
    public DateTime Created { get; set; }
}
public class CartLine
{
    public Product P { get; set; } = null!;
    public int Qty { get; set; }
    public int Sum => P.Price * Qty;
    public string SumText => $"{Sum:N0} ₽";
}

/// <summary>Локальная база данных SQLite: каталог, корзина, заказы.</summary>
public static class Store
{
    public static readonly string[] Categories = { "Протеин", "Креатин", "Аминокислоты", "Гейнеры", "Витамины", "Батончики" };
    public static string? PendingCategory, PendingQuery;   // фильтр, переданный с главного экрана
    static SQLiteAsyncConnection? db;

    static async Task<SQLiteAsyncConnection> Conn()
    {
        if (db != null) return db;
        var c = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "athlet.db3"));
        await c.CreateTableAsync<Product>();
        await c.CreateTableAsync<CartItem>();
        await c.CreateTableAsync<Order>();
        if (await c.Table<Product>().CountAsync() == 0) await c.InsertAllAsync(Seed());
        return db = c;
    }

    public static async Task<List<Product>> Products(string? cat = null, string? q = null)
    {
        var l = await (await Conn()).Table<Product>().ToListAsync();
        if (!string.IsNullOrEmpty(cat) && cat != "Все") l = l.Where(p => p.Category == cat).ToList();
        if (!string.IsNullOrWhiteSpace(q))
            l = l.Where(p => (p.Name + " " + p.Brand + " " + p.Category).Contains(q.Trim(), StringComparison.CurrentCultureIgnoreCase)).ToList();
        return l;
    }
    public static async Task<Product?> Get(int id) => await (await Conn()).FindAsync<Product>(id);

    public static async Task AddToCart(int id)
    {
        var c = await Conn(); var i = await c.FindAsync<CartItem>(id);
        if (i == null) await c.InsertAsync(new CartItem { ProductId = id, Qty = 1 });
        else { i.Qty++; await c.UpdateAsync(i); }
    }
    public static async Task SetQty(int id, int qty)
    {
        var c = await Conn();
        if (qty <= 0) await c.DeleteAsync<CartItem>(id);
        else await c.UpdateAsync(new CartItem { ProductId = id, Qty = qty });
    }
    public static async Task<List<CartLine>> Cart()
    {
        var c = await Conn(); var res = new List<CartLine>();
        foreach (var i in await c.Table<CartItem>().ToListAsync())
            if (await c.FindAsync<Product>(i.ProductId) is Product p) res.Add(new CartLine { P = p, Qty = i.Qty });
        return res;
    }
    public static async Task<int> PlaceOrder(Order o)
    {
        var c = await Conn(); await c.InsertAsync(o); await c.DeleteAllAsync<CartItem>(); return o.Id;
    }

    static List<Product> Seed()
    {
        Product P(string cat, string brand, string name, string emoji, string w, int price, string d, string comp) =>
            new() { Category = cat, Brand = brand, Name = name, Emoji = emoji, Weight = w, Price = price, Description = d, Composition = comp };
        return new()
        {
            P("Протеин","ATHLET","Whey Protein Gold","🥛","900 г",1890,"Сывороточный протеин для набора и поддержания мышечной массы. 24 г белка в порции.","Концентрат сывороточного белка, лецитин, ароматизатор, подсластитель."),
            P("Протеин","PowerLab","Whey Isolate","🥤","2 кг",4590,"Изолят с минимумом жиров и лактозы, быстро усваивается после тренировки.","Изолят сывороточного белка, ароматизатор, подсластитель."),
            P("Протеин","IronFuel","Casein Night","🌙","900 г",2190,"Медленный казеиновый протеин для приёма перед сном.","Мицеллярный казеин, какао, загуститель."),
            P("Креатин","ATHLET","Creatine Monohydrate","💪","300 г",1190,"Моногидрат креатина для силы и выносливости. 5 г в порции.","Креатин моногидрат 100%."),
            P("Креатин","PowerLab","Creatine Caps","💊","120 капсул",990,"Креатин в капсулах — удобно брать с собой.","Креатин моногидрат, капсула (желатин)."),
            P("Аминокислоты","IronFuel","BCAA 2:1:1","⚡","300 г",1390,"Лейцин, изолейцин и валин для восстановления во время тренировки.","L-лейцин, L-изолейцин, L-валин, лимонная кислота, ароматизатор."),
            P("Аминокислоты","PowerLab","Glutamine","🔥","250 г",1090,"Глютамин для восстановления после нагрузок.","L-глютамин 100%."),
            P("Гейнеры","ATHLET","Mass Gainer 3000","🏋️","3 кг",2790,"Белково-углеводная смесь для набора массы. 600 ккал в порции.","Мальтодекстрин, концентрат сывороточного белка, какао, витамины."),
            P("Гейнеры","IronFuel","Hard Gainer","🍌","1,5 кг",1890,"Гейнер для людей с быстрым обменом веществ.","Углеводный комплекс, белки молока, ароматизатор."),
            P("Витамины","ATHLET","Omega-3","🐟","90 капсул",1090,"Рыбий жир: ЭПК и ДГК для сердца и суставов.","Рыбий жир, желатин, глицерин, витамин E."),
            P("Витамины","PowerLab","Multivitamin Sport","🍊","60 таблеток",890,"Витаминно-минеральный комплекс для активных людей.","Витамины A, C, D, E, группа B, цинк, магний."),
            P("Батончики","IronFuel","Protein Bar","🍫","60 г",140,"Протеиновый батончик, 20 г белка, без добавленного сахара.","Белок молока, орехи, подсластитель, какао."),
        };
    }
}

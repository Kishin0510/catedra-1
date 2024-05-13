using Catedra1DWB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Books"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var ebooks = app.MapGroup("api/ebook");

// TODO: Add more routes
ebooks.MapPost("/", CreateEBookAsync);
ebooks.MapGet("genre={genre}&author{author}&format={format}", GetAllEBookAsync);
ebooks.MapPut("{id}", UpdateEBookAsync);
ebooks.MapPut("{id}/change-availability", UpdateStatusEbookAsync);
ebooks.MapPut("{id}/increment-stock", IncrementStockEBookAsync);
ebooks.MapPost("/purchase", BuyEBooksAsync);
ebooks.MapDelete("{id}",DeleteEbookAsync);

app.Run();

// TODO: Add more methods
static async Task<IResult> CreateEBookAsync(DataContext _context, CreateBookDto createBook)
{
    var oldBook = _context.Books.Where(b => b.Title == createBook.Title && b.Author == createBook.Author).FirstOrDefault();
    if(oldBook != null){
        return Results.BadRequest();
    }
    var newbook = new Book{ Title = createBook.Title, Author = createBook.Author, Genre = createBook.Genre, Format = createBook.Format, Price = createBook.Price, IsAvailable = true, Stock = 0};
    
    _context.Books.Add(newbook);
    await _context.SaveChangesAsync();
    return Results.Created($"/books/{newbook.Id}", newbook);
    
}

static async Task<IEnumerable<Book>> GetAllEBookAsync(DataContext _context, string genre, string author, string format){
    if(genre != null && author != null && format != null){
        var books = await _context.Books.Where(b => b.Genre == genre && b.Author == author && b.Format == format).ToListAsync();
        return books;
    }
    else if (genre != null && author != null && format == null){
        var books = await _context.Books.Where(b => b.Genre == genre && b.Author == author).ToListAsync();
        return books;
    }
    else if (genre != null && author == null && format == null){
        var books = await _context.Books.Where(b => b.Genre == genre).ToListAsync();
        return books;
    }
    else if (genre == null && author != null && format == null){
        var books = await _context.Books.Where(b => b.Author == author).ToListAsync();
        return books;
    }
    else if (genre == null && author == null && format != null){
        var books = await _context.Books.Where(b => b.Format == format).ToListAsync();
        return books;
    }
    else {
        return null;
    }
}
static async Task<IResult> UpdateEBookAsync(DataContext _context, int id, CreateBookDto updatedBook){
    var oldbook = await _context.Books.FindAsync(id);
    if (oldbook is null){
        return Results.NotFound();
    }
    oldbook.Title = updatedBook.Title;
    oldbook.Author = updatedBook.Author;
    oldbook.Genre = updatedBook.Genre;
    oldbook.Price = updatedBook.Price;
    oldbook.Format = updatedBook.Format;

    await _context.SaveChangesAsync();
    return Results.NoContent();
}

static async Task<IResult> UpdateStatusEbookAsync (DataContext _context, int id){
    var oldbook = await _context.Books.FindAsync(id);
    if(oldbook is null){
        return Results.NotFound();
    }
    if(oldbook.IsAvailable){
        oldbook.IsAvailable = false;
    } else {
        oldbook.IsAvailable = true;
    }
    await _context.SaveChangesAsync();
    return Results.NoContent();
}
static async Task<IResult> IncrementStockEBookAsync (DataContext _context, int id, [FromBody] int newstock){
    var oldBook = await _context.Books.FindAsync(id);
    if(oldBook is null){
        return Results.NotFound();
    }
    oldBook.Stock = oldBook.Stock + newstock;
    await _context.SaveChangesAsync();
    return Results.Ok();
}

static async Task<IResult> BuyEBooksAsync (DataContext _context, int id, [FromBody] int copies,int totalPrice){
    var oldbook = await _context.Books.FindAsync(id);
    if(oldbook is null) {
        return Results.NotFound();
    }
    var totalPriceCalc = oldbook.Price * copies;
    if(totalPriceCalc == totalPrice){
        oldbook.Stock = oldbook.Stock - copies;
        await _context.SaveChangesAsync();
        return Results.Ok();
    }
    return Results.BadRequest();
}

static async Task<IResult> DeleteEbookAsync (DataContext _context, int id){
    var oldbook = await _context.Books.FindAsync(id);
    if(oldbook is null){
        return Results.NotFound();
    }
    _context.Remove(oldbook);
    await _context.SaveChangesAsync();
    return Results.NoContent();
}



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "books-by-year",
    pattern: "books/year/{year:int:min(1450)}",
    defaults: new { controller = "Book", action = "ByYear" });

app.MapControllerRoute(
    name: "books-by-isbn",
    pattern: "books/isbn/{isbn:regex(^\\d{{13}}$)}",
    defaults: new { controller = "Book", action = "ByIsbn" });

app.MapControllerRoute(
    name: "docs",
    pattern: "docs/{*path}",
    defaults: new { controller = "Book", action = "Docs" });

//Lab 3
app.MapControllerRoute(
    name: "admin-area",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Book}/{action=Index}/{id?}");

app.Run();
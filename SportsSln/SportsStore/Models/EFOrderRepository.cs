using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace SportsStore.Models {
    public class EFOrderRepository : IOrderRepository {
        private StoreDbContext context;
        public EFOrderRepository (StoreDbContext ctx) {
            context = ctx;
        }

        public IQueryable<Order> Orders => context.Orders
            .Include (o => o.Lines)
            .ThenInclude (l => l.Product);

        public void SaveOrder (Order order) {
            // исключение из генерируемого sql-запроса
            // информации про товары
            // (для этого заменяем все модели "товар в корзине"
            // на "товар" и указываем получившийся список,
            // чтобы не было попытки добавить в базу данный "товар",
            // которые там уже есть)
            context.AttachRange (order.Lines.Select (l => l.Product));
            // если заказ сохраняется в БД впервые - выполняем построение
            // команды вставки строки в таблицу методом Add
            if (order.OrderID == 0) {
                context.Orders.Add (order);
            }
            // иначе ничего не делаем, и это приводит к формированию команды обновления
            // в момент вызова метода SaveChanges
            context.SaveChanges ();
        }
    }
}
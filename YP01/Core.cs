using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YP01
{
    /// <summary>
    /// Класс, предоставляющий доступ до базы данных и хранящий глобальное состояние.
    /// Так как `db` определён только здесь, изменение типа этого поля (что
    /// часто происходит при переподключении базы данных) требует изменения
    /// кода только здесь, а не во всех местах использования базы данных.
    /// </summary>
    public class Core
    {
        /// <summary>
        /// Экземпляр соединения до базы данных
        /// </summary>
        // Исправлено: PraktiqueEntities2 ? YP11DANEntities
        public static YP11DANEntities db = new YP11DANEntities();

        /// <summary>
        /// Текущий пользователь, вошедший в приложение
        /// </summary>
        // Исправлено: User ? Accounts
        public static Accounts current_user { get; set; }
    }
}
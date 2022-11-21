using System;
using System.Threading.Tasks;

namespace TroedelMarkt {
    public abstract class AbstractHttpManager {
        public abstract Uri BaseUri { get; }

        /*
         * This is an asynchronous method.
         * It returns a new instance of this class that is already authenticated
         * Raises:
         *  UnauthorizedException: The password is incorrect.
         *  NotFoundException: The server specified does not reply.
        */
        public abstract Task<AbstractHttpManager> NewAuthenticated(
            string ip_or_hostname,
            int? port,
            string password
        );

        /*
         * This is an asynchronous method.
         * It returns an updated instance of the trader that was passed in
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
         *  DuplicateException: A trader with this trader.TraderID already exists.
        */
        public abstract Task<Trader> CreateNewTrader(
            Trader trader
        );
        public abstract Task<Trader> CreateNewTrader(
            string traderID,
            string name,
            decimal rate
        );

        /*
         * This is an asynchronous method.
         * It returns an array of all traders currently registered with the server
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
        */
        public abstract Task<Trader[]> GetAllTraders();

        /*
         * This is an asynchronous method.
         * It returns a newly created Trader instance with all up-to-date information
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
         *  NotFoundException: No trader with this id exists.
        */
        public abstract Task<Trader> GetTrader(
            string traderID
        );

        /*
         * This is an asynchronous method.
         * It returns the trader that was just deleted with all the previously existing information.
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
         *  NotFoundException: No trader with the specified id exists.
         *  DeletionOrderException: The selected trader has stored balance and cannot be deleted.
        */
        public abstract Task<Trader> DeleteTrader(string traderID);
        public abstract Task<Trader> DeleteTrader(Trader trader);

        /*
         * This is an asynchronous method.
         * It returns the Trader instance passed in with all applied updates
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
         *  NotFoundException: No trader with trader.TraderID exists.
        */
        public abstract Task<Trader> UpdateTrader(Trader trader);

        /*
         * This is an asynchronous method.
         * It returns an array of all the Traders that have been changed.
         * These are new objects, not old ones being updated.
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
        */
        public abstract Task<Trader[]> SellItems(System.Collections.Generic.List<TransactionItem> items);
        public abstract Task<Trader[]> SellItems(TransactionItem[] items);

        /*
         * This is an asynchronous method.
         * It returns a stream of the csv-file.
         * Raises:
         *  UnauthorizedException: The client is not authenticated.
        */
        public abstract Task<System.IO.Stream> ExportCSV();
    }
}
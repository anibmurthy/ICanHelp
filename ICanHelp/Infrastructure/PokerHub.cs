using ICanHelp.Contracts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICanHelp.Infrastructure
{
    public class PokerHub : Hub
    {
        private IPointingPokerRepository _pokerRepo;
        public PokerHub(IPointingPokerRepository pokerRepo)
        {
            _pokerRepo = pokerRepo;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SubscribeToBoard(int boardId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, boardId.ToString());
            await Clients.Caller.SendAsync("Message", "Added to board successfully!");
            await Clients.Caller.SendAsync("SetConnectionId", Context.ConnectionId);
        }

        public async Task RemoveFromBoard(int userId)
        {
            await _pokerRepo.RemoveUser(userId, Context.ConnectionId);
        }
    }
}

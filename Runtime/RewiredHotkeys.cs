using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.UI
{
    public class RewiredHotkeys : MonoBehaviour
    {
        private static RewiredHotkeys _instance;

        [SerializeField]
        private bool _dontDestroyOnLoad = true;

        private static bool _isReady;
        public static bool isReady => _isReady;

        private static Player _systemPlayer = null;
        public static Player systemPlayer => _systemPlayer;

        private static readonly List<Player> _players = new List<Player>();
        public static IReadOnlyList<Player> players => _players;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => ReInput.isReady);

            var systemPlayer = ReInput.players.SystemPlayer;
            _systemPlayer = new Player(systemPlayer);
            _players.Add(_systemPlayer);

            _isReady = true;
        }

        private void Update()
        {
            for (int i = 0; i < _players.Count; ++i)
            {
                _players[i].Update();
            }
        }

        public static Player AddPlayer(Rewired.Player player)
        {
            if (player == null)
                return null;

            var newPlayer = new Player(player);
            _players.Add(newPlayer);
            return newPlayer;
        }

        public static bool RemovePlayer(Player player)
        {
            if (player == null)
                return false;

            if (!_players.Contains(player))
                return false;

            _players.Remove(player);
            player.Dispose();
            return true;
        }

        public class Player : IDisposable
        {
            private bool _isDisposed;
            public bool isDisposed => _isDisposed;

            private Rewired.Player _player;
            public Rewired.Player player => _player;

            private Controller _lastActiveController;
            public Controller lastActiveController => _lastActiveController;

            public delegate void OnControllerChangedDelegate(Controller controller);
            public OnControllerChangedDelegate onControllerChanged;

            public Player(Rewired.Player player)
            {
                _player = player;
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                _player = null;
                _lastActiveController = null;
            }

            public void Update()
            {
                var ctrl = _player.controllers.GetLastActiveController();
                SetController(ctrl);
            }

            private void SetController(Controller ctrl)
            {
                if (_lastActiveController == ctrl)
                    return;

                if (_lastActiveController != null)
                {
                    //do nothing
                    _lastActiveController = null;
                }

                _lastActiveController = ctrl;

                if (_lastActiveController != null)
                {
                    //do nothing
                }

                 onControllerChanged?.Invoke(_lastActiveController);
            }
        }
    }
}

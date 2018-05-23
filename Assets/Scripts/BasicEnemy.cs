using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class BasicEnemy: MovingObject
    {
        public Player Player { get; set; }
        private float _minDistanceToPlayer = 0.5f;

        public float Health { get; set; }

        public override void Start()
        {
            base.Start();
            Health = 100;
            Player = PrefabRepository.Instance.Player.GetComponent<Player>();
        }

        void FixedUpdate()
        {
            Vector3 diffVec = Player.transform.position - transform.position;

            float direction = Math.Abs(diffVec.x) / diffVec.x;
            float boundsWidth = _boxCollider.bounds.size.x;

            if (_isGrounded)
            {
                if (Math.Abs(diffVec.x) > _minDistanceToPlayer)
                    _rb.AddForce(Vector2.right * _speedX * direction);

                if (Math.Abs(_rb.velocity.x) > _maxSpeedX)
                {
                    _rb.velocity = new Vector2((Math.Abs(_rb.velocity.x) / _rb.velocity.x) * _maxSpeedX,
                        _rb.velocity.y);
                }
            }

            _boxCollider.enabled = false;
            RaycastHit2D hit = Physics2D.Linecast(transform.position,
                (Vector2)transform.position + new Vector2(boundsWidth * direction * 1.5f, 0));
            _boxCollider.enabled = true;
            if (hit.transform != null)
            {
                Debug.Log("BLOCK IN FRONT!!!" + hit.transform.tag);
                float directionOfJump = _rb.velocity.x == 0 ? 0 : (Math.Abs(_rb.velocity.x) / _rb.velocity.x);
                if (_isGrounded)
                {
                    
                    _rb.velocity = new Vector2(directionOfJump * (_maxSpeedX),
                        _rb.velocity.y);
                    _rb.AddForce(Vector2.up * _jumpForce * 1.1f);
                }
                else
                {
                    _rb.velocity = new Vector2(directionOfJump * (_maxSpeedX / 4),
                        _rb.velocity.y);
                }
            }
        }
    }
}

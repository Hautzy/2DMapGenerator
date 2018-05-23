using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MovingObject: MonoBehaviour
    {
        protected float _speedX = 20f;
        protected float _maxSpeedX = 3f;
        protected float _jumpForce = 250f;
        protected Rigidbody2D _rb;
        protected BoxCollider2D _boxCollider;
        protected bool _isGrounded;

        public virtual void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _isGrounded = true;
        }

        public bool CheckIfBlockBeneath(string curTag)
        {
            float offset = _boxCollider.bounds.size.y / 2 * 1.1f;
            _boxCollider.enabled = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, offset);
            _boxCollider.enabled = true;
            if (hit.transform != null && hit.transform.tag == curTag)
            {
                return true;
            }
            return false;
        }

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
            _isGrounded = CheckIfBlockBeneath("Block");
        }

        public virtual void OnCollisionExit2D(Collision2D collision)
        {
            _isGrounded = false;
        }
    }
}

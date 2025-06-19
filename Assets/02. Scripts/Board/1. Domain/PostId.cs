using System;

public readonly struct PostId : IEquatable<PostId>
{
    public string Value { get; }

    public PostId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("PostId는 비어 있을 수 없습니다.");

        Value = value;
    }

    public static PostId NewId() => new PostId(Guid.NewGuid().ToString());

    // string → PostId
    public static implicit operator PostId(string value) => new PostId(value);

    // PostId → string
    public static implicit operator string(PostId id) => id.Value;

    public override string ToString() => Value;

    public bool Equals(PostId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is PostId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}

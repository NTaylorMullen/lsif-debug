export class CaseInsensitiveMap<T, U> extends Map<T, U> {
	get(key: T): U | undefined {
		if (typeof key === 'string') {
			const lowerCasedKey = key.toLowerCase();
			for (const storedKey of this.keys()) {
				if ((storedKey as any).toLowerCase() === lowerCasedKey) {
					return super.get(storedKey as any as T);
				}
			}
		}

		return super.get(key);
	}

	has(key: T): boolean {
		if (typeof key === 'string') {
			const lowerCasedKey = key.toLowerCase();
			for (const storedKey of this.keys()) {
				if ((storedKey as any).toLowerCase() === lowerCasedKey) {
					return super.has(storedKey as any as T);
				}
			}
		}

		return super.has(key);
	}
}
